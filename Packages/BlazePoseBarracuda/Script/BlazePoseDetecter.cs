using UnityEngine;
using Mediapipe.PoseDetection;
using Mediapipe.PoseLandmark;

namespace Mediapipe.BlazePose{
    public class BlazePoseDetecter: System.IDisposable
    {
        #region public variables
        /*
        Pose landmark result Buffer.
        'outputBuffer' is array of float4 type.

        0~32 index datas are pose landmark.
            Check below Mediapipe document about relation between index and landmark position.
            https://google.github.io/mediapipe/solutions/pose#pose-landmark-model-blazepose-ghum-3d
            Each data factors are
            x: x cordinate value of pose landmark ([0, 1]).
            y: y cordinate value of pose landmark ([0, 1]).
            z: Landmark depth with the depth at the midpoint of hips being the origin.
               The smaller the value the closer the landmark is to the camera. ([0, 1]).
               This value is full body mode only.
               **The use of this value is not recommended beacuse in development.**
            w: The score of whether the landmark position is visible ([0, 1]).
        
        33 index data is the score whether human pose is visible ([0, 1]).
        This data is (score, 0, 0, 0).
        */
        public ComputeBuffer outputBuffer;
        /*
        Pose world landmark result Buffer.
            'worldLandmarkBuffer' is array of float4 type.
            0~32 index datas are pose world landmark.

            Each data factors are
            x, y and z: Real-world 3D coordinates in meters with the origin at the center between hips.
            w: The score of whether the world landmark position is visible ([0, 1]).

            33 index data is the score whether human pose is visible ([0, 1]).
            This data is (score, 0, 0, 0).
        */
        public ComputeBuffer worldLandmarkBuffer;
        // Count of pose landmark vertices.
        public int vertexCount => landmarker.vertexCount;
        #endregion

        #region constant number
        // Pose detection neural network model's input size.
        const int DETECTION_INPUT_IMAGE_SIZE = 128;
        // Pose landmark neural network model's input size.
        const int LANDMARK_INPUT_IMAGE_SIZE = 256;
        const int rvfWindowMaxCount = 5;
        #endregion

        #region private variable
        PoseDetecter detecter;
        PoseLandmarker landmarker;
        ComputeShader cs;
        RenderTexture letterboxTexture, cropedTexture, rvfWindow, rvfWindowWorld;
        ComputeBuffer poseRegionBuffer, lastValueScale, lastValueScaleWorld;
        int rvfWindowCount;
        #endregion

        #region public method
        public BlazePoseDetecter(BlazePoseResource resource, BlazePoseModel blazePoseModel){
            cs = resource.cs;
            detecter = new PoseDetecter(resource.detectionResource);
            landmarker = new PoseLandmarker(resource.landmarkResource, (PoseLandmarkModel)blazePoseModel);

            // `letterboxTexture` is set readable/writable RenderTexture.
            letterboxTexture = new RenderTexture(DETECTION_INPUT_IMAGE_SIZE, DETECTION_INPUT_IMAGE_SIZE, 0, RenderTextureFormat.ARGB32);
            letterboxTexture.enableRandomWrite = true;
            letterboxTexture.Create();

            // `cropedTexture` is set readable/writable RenderTexture.
            cropedTexture = new RenderTexture(LANDMARK_INPUT_IMAGE_SIZE, LANDMARK_INPUT_IMAGE_SIZE, 0, RenderTextureFormat.ARGB32);
            cropedTexture.enableRandomWrite = true;
            cropedTexture.Create();

            // `rvfWindow` and 'rvfWindowWorld' is set readable/writable RenderTexture.
            rvfWindow = new RenderTexture(rvfWindowMaxCount, vertexCount, 0, RenderTextureFormat.ARGBFloat);
            rvfWindow.enableRandomWrite = true;
            rvfWindow.Create();
            rvfWindowWorld = new RenderTexture(rvfWindowMaxCount, vertexCount, 0, RenderTextureFormat.ARGBFloat);
            rvfWindowWorld.enableRandomWrite = true;
            rvfWindowWorld.Create();
            rvfWindowCount =  0;

            poseRegionBuffer = new ComputeBuffer(1, sizeof(float) * 24);
            lastValueScale = new ComputeBuffer(landmarker.vertexCount, sizeof(float) * 3);
            lastValueScaleWorld = new ComputeBuffer(landmarker.vertexCount, sizeof(float) * 3);

            // Output length is pose landmark count(33) + human exist flag(1).
            outputBuffer = new ComputeBuffer(landmarker.vertexCount + 1, sizeof(float) * 4);
            worldLandmarkBuffer = new ComputeBuffer(landmarker.vertexCount + 1, sizeof(float) * 4);
        }

        // Process pipeline is refered https://google.github.io/mediapipe/solutions/pose#ml-pipeline.
        // Check above URL or BlazePose paper(https://arxiv.org/abs/2006.10204) for details.
        public void ProcessImage(
            Texture inputTexture, 
            BlazePoseModel blazePoseModel, 
            float poseThreshold = 0.75f, 
            float iouThreshold = 0.3f)
        {
            // Letterboxing scale factor
            var scale = new Vector2(
                Mathf.Max((float)inputTexture.height / inputTexture.width, 1),
                Mathf.Max(1, (float)inputTexture.width / inputTexture.height)
            );

            // Image scaling and padding
            // Output image is letter-box image.
            // For example, top and bottom pixels of `letterboxTexture` are black if `inputTexture` size is 1920(width)*1080(height)
            cs.SetInt("_letterboxWidth", DETECTION_INPUT_IMAGE_SIZE);
            cs.SetVector("_spadScale", scale);
            cs.SetTexture(0, "_letterboxInput", inputTexture);
            cs.SetTexture(0, "_letterboxTexture", letterboxTexture);
            cs.Dispatch(0, DETECTION_INPUT_IMAGE_SIZE / 8, DETECTION_INPUT_IMAGE_SIZE / 8, 1);

            // Predict Pose detection.
            detecter.ProcessImage(letterboxTexture, poseThreshold, iouThreshold);

            // Update Pose Region from detected results.
            cs.SetFloat("_deltaTime", Time.deltaTime);
            cs.SetInt("_upperBodyOnly", 0);
            cs.SetBuffer(1, "_poseDetections", detecter.outputBuffer);
            cs.SetBuffer(1, "_poseDetectionCount", detecter.countBuffer);
            cs.SetBuffer(1, "_poseRegions", poseRegionBuffer);
            cs.Dispatch(1, 1, 1, 1);

            // Scale and pad to letter-box image and crop pose region from letter-box image.
            cs.SetTexture(2, "_inputTexture", inputTexture);
            cs.SetBuffer(2, "_cropRegion", poseRegionBuffer);
            cs.SetTexture(2, "_cropedTexture", cropedTexture);
            cs.Dispatch(2, LANDMARK_INPUT_IMAGE_SIZE / 8, LANDMARK_INPUT_IMAGE_SIZE / 8, 1);

            // Predict pose landmark.
            landmarker.ProcessImage(cropedTexture, (PoseLandmarkModel)blazePoseModel);

            float fps = 1.0f / Time.unscaledDeltaTime;
            float velocity_scale = 3.0f / fps;

            // Map to cordinates of `inputTexture` from pose landmarks on croped letter-box image.
            cs.SetInt("_isWorldProcess", 0);
            cs.SetInt("_keypointCount", landmarker.vertexCount);
            cs.SetFloat("_postDeltatime", Time.deltaTime);
            cs.SetFloat("_velocity_scale", velocity_scale);
            cs.SetInt("_rvfWindowCount", rvfWindowCount);
            cs.SetBuffer(3, "_postInput", landmarker.outputBuffer);
            cs.SetBuffer(3, "_postRegion", poseRegionBuffer);
            cs.SetTexture(3, "_postRvfWindow", rvfWindow);
            cs.SetBuffer(3, "_postLastValueScale", lastValueScale);
            cs.SetBuffer(3, "_postOutput", outputBuffer);
            cs.Dispatch(3, 1, 1, 1);

            // Map to cordinates of `inputTexture` from pose landmarks on croped letter-box image for 3D world landmarks.
            cs.SetInt("_isWorldProcess", 1);
            cs.SetInt("_keypointCount", landmarker.vertexCount);
            cs.SetFloat("_postDeltatime", Time.deltaTime);
            cs.SetFloat("_velocity_scale", velocity_scale);
            cs.SetInt("_rvfWindowCount", rvfWindowCount);
            cs.SetBuffer(3, "_postInput", landmarker.worldLandmarkBuffer);
            cs.SetBuffer(3, "_postRegion", poseRegionBuffer);
            cs.SetTexture(3, "_postRvfWindow", rvfWindowWorld);
            cs.SetBuffer(3, "_postLastValueScale", lastValueScaleWorld);
            cs.SetBuffer(3, "_postOutput", worldLandmarkBuffer);
            cs.Dispatch(3, 1, 1, 1);

            rvfWindowCount = Mathf.Min(rvfWindowCount + 1, rvfWindowMaxCount);
        }

        public void Dispose(){
            detecter.Dispose();
            landmarker.Dispose();
            letterboxTexture.Release();
            cropedTexture.Release();
            rvfWindow.Release();
            rvfWindowWorld.Release();
            poseRegionBuffer.Dispose();
            lastValueScale.Dispose();
            lastValueScaleWorld.Dispose();
            outputBuffer.Dispose();
            worldLandmarkBuffer.Dispose();
        }
        #endregion
    }
}
