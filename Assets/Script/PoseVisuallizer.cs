using UnityEngine;
using UnityEngine.UI;
using Mediapipe.BlazePose;

public class PoseVisuallizer : MonoBehaviour
{
    [SerializeField] WebCamInput webCamInput;
    [SerializeField] Shader shader;
    [SerializeField] BlazePoseResource blazePoseResource;
    [SerializeField] bool isUpperBodyOnly;

    Material material;
    BlazePoseDetecter detecter;

    public RawImage image;


    void Start(){
        material = new Material(shader);
        detecter = new BlazePoseDetecter(blazePoseResource, isUpperBodyOnly);
        image.texture = detecter.cropedTexture;
    }

    void LateUpdate(){
        // Predict pose by neural network model.
        detecter.ProcessImage(webCamInput.inputImageTexture, isUpperBodyOnly);
    } 

    void OnRenderObject(){
        material.SetPass(0);
        // Set predicted pose landmark results.
        material.SetBuffer("_vertices", detecter.outputBuffer);
        // Draw (25 or 33) landmark points.
        Graphics.DrawProceduralNow(MeshTopology.Lines, 4, detecter.vertexCount);
    }

    void OnApplicationQuit(){
        detecter.Dispose();
    }
}
