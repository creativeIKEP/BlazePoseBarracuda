using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.PoseDetection;
using Mediapipe.PoseLandmark;

namespace Mediapipe.BlazePose{
    public class BlazePoseDetecter: System.IDisposable
    {
        const int DETECTION_INPUT_IMAGE_SIZE = 128;
        const int LANDMARK_INPUT_IMAGE_SIZE = 256;
        PoseDetecter detecter;
        PoseLandmarker landmarker;
        ComputeShader cs;
        RenderTexture letterboxTexture, cropedTexture;
        ComputeBuffer poseRegionBuffer;
        public ComputeBuffer outputBuffer;
        public int vertexCount => landmarker.vertexCount;

        public BlazePoseDetecter(BlazePoseResource resource, bool isUpperBody){
            cs = resource.cs;
            detecter = new PoseDetecter(resource.detectionResource);
            landmarker = new PoseLandmarker(resource.landmarkResource, isUpperBody);

            letterboxTexture = new RenderTexture(DETECTION_INPUT_IMAGE_SIZE, DETECTION_INPUT_IMAGE_SIZE, 0, RenderTextureFormat.ARGB32);
            letterboxTexture.enableRandomWrite = true;
            letterboxTexture.Create();

            cropedTexture = new RenderTexture(LANDMARK_INPUT_IMAGE_SIZE, LANDMARK_INPUT_IMAGE_SIZE, 0, RenderTextureFormat.ARGB32);
            cropedTexture.enableRandomWrite = true;
            cropedTexture.Create();

            poseRegionBuffer = new ComputeBuffer(1, sizeof(float) * 24);
            outputBuffer = new ComputeBuffer(landmarker.vertexCount * 2, sizeof(float) * 4);
        }

        public void ProcessImage(
            Texture inputTexture, 
            bool isUpperBody, 
            float poseThreshold = 0.75f, 
            float iouThreshold = 0.3f)
        {
            // Letterboxing scale factor
            var scale = new Vector2(
                Mathf.Max((float)inputTexture.height / inputTexture.width, 1),
                Mathf.Max(1, (float)inputTexture.width / inputTexture.height)
            );

            // Image scaling and padding
            cs.SetInt("_letterboxWidth", DETECTION_INPUT_IMAGE_SIZE);
            cs.SetVector("_spadScale", scale);
            cs.SetTexture(0, "_letterboxInput", inputTexture);
            cs.SetTexture(0, "_letterboxTexture", letterboxTexture);
            cs.Dispatch(0, DETECTION_INPUT_IMAGE_SIZE / 8, DETECTION_INPUT_IMAGE_SIZE / 8, 1);
        
            detecter.ProcessImage(letterboxTexture, poseThreshold, iouThreshold);

            cs.SetFloat("_deltaTime", Time.deltaTime);
            cs.SetInt("_upperBodyOnly", (isUpperBody ? 1 : 0));
            cs.SetBuffer(1, "_poseDetections", detecter.outputBuffer);
            cs.SetBuffer(1, "_poseDetectionCount", detecter.countBuffer);
            cs.SetBuffer(1, "_poseRegions", poseRegionBuffer);
            cs.Dispatch(1, 1, 1, 1);

            cs.SetVector("_spad_scale", scale);
            cs.SetTexture(2, "_inputTexture", inputTexture);
            cs.SetBuffer(2, "_cropRegion", poseRegionBuffer);
            cs.SetTexture(2, "_cropedTexture", cropedTexture);
            cs.Dispatch(2, LANDMARK_INPUT_IMAGE_SIZE / 8, LANDMARK_INPUT_IMAGE_SIZE / 8, 1);

            landmarker.ProcessImage(cropedTexture, isUpperBody);

            if(landmarker.vertexCount * 2 != outputBuffer.count){
                outputBuffer?.Dispose();
                outputBuffer = new ComputeBuffer(landmarker.vertexCount * 2, sizeof(float) * 4);
                if(isUpperBody) cs.EnableKeyword("UPPER_BODY");
                else cs.DisableKeyword("UPPER_BODY");
            }

            cs.SetFloat("_post_dt", Time.deltaTime);
            cs.SetFloat("_post_scale", scale.y);
            cs.SetBuffer(3, "_post_input", landmarker.outputBuffer);
            cs.SetBuffer(3, "_post_region", poseRegionBuffer);
            cs.SetBuffer(3, "_post_output", outputBuffer);
            cs.Dispatch(3, 1, 1, 1);
        }

        public void Dispose(){
            detecter.Dispose();
            landmarker.Dispose();
            letterboxTexture.Release();
            cropedTexture.Release();
            poseRegionBuffer.Dispose();
            outputBuffer.Dispose();
        }
    }
}