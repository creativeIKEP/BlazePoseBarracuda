using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.BlazePose;

public class PoseVisuallizer : MonoBehaviour
{
    [SerializeField] WebCamInput webCamInput;
    [SerializeField] RawImage inputImageUI;
    [SerializeField] Shader shader;
    [SerializeField] BlazePoseResource blazePoseResource;
    [SerializeField] bool isUpperBodyOnly;

    Material material;
    BlazePoseDetecter detecter;
    const int UPPER_BODY_LINE_NUM = 25;
    const int FULL_BODY_LINE_NUM = 35;
    readonly List<Vector4> linePair = new List<Vector4>{
        new Vector4(0, 1), new Vector4(1, 2), new Vector4(2, 3), new Vector4(3, 7), new Vector4(0, 4), 
        new Vector4(4, 5), new Vector4(5, 6), new Vector4(6, 8), new Vector4(9, 10), new Vector4(11, 12), 
        new Vector4(11, 13), new Vector4(13, 15), new Vector4(15, 17), new Vector4(17, 19), new Vector4(19, 15), 
        new Vector4(15, 21), new Vector4(12, 14), new Vector4(14, 16), new Vector4(16, 18), new Vector4(18, 20), 
        new Vector4(20, 16), new Vector4(16, 22), new Vector4(11, 23), new Vector4(12, 24), new Vector4(23, 24), 
        new Vector4(23, 25), new Vector4(25, 27), new Vector4(27, 29), new Vector4(29, 31), new Vector4(31, 27), 
        new Vector4(24, 26), new Vector4(26, 28), new Vector4(28, 30), new Vector4(30, 32), new Vector4(32, 28)
    };


    void Start(){
        material = new Material(shader);
        detecter = new BlazePoseDetecter(blazePoseResource, isUpperBodyOnly);
    }

    void LateUpdate(){
        inputImageUI.texture = webCamInput.inputImageTexture;

        // Predict pose by neural network model.
        detecter.ProcessImage(webCamInput.inputImageTexture, isUpperBodyOnly);
    } 

    void OnRenderObject(){
        var w = inputImageUI.rectTransform.rect.width;
        var h = inputImageUI.rectTransform.rect.height;

        material.SetPass(0);
        // Set predicted pose landmark results.
        material.SetBuffer("_vertices", detecter.outputBuffer);
        material.SetVector("_uiScale", new Vector2(w, h));
        // Draw (25 or 33) landmark points.
        Graphics.DrawProceduralNow(MeshTopology.Lines, 4, detecter.vertexCount);

        material.SetPass(1);
        material.SetVectorArray("_linePair", linePair);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 2, isUpperBodyOnly ? UPPER_BODY_LINE_NUM : FULL_BODY_LINE_NUM);
    }

    void OnApplicationQuit(){
        detecter.Dispose();
    }
}
