using UnityEngine;
using Mediapipe.PoseDetection;
using Mediapipe.PoseLandmark;

namespace Mediapipe.BlazePose{
    [CreateAssetMenu(fileName = "BlazePose", menuName = "ScriptableObjects/BlazePose Resource")]
    public class BlazePoseResource : ScriptableObject
    {
        public PoseDetectionResource detectionResource;
        public PoseLandmarkResource landmarkResource;
        public ComputeShader cs;
    }
}