Shader "Hidden/BlazePose/Visualizer"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    StructuredBuffer<float4> _vertices;

    struct v2f{
        float4 position: SV_POSITION;
        float4 color: COLOR;
    };

    v2f Vertex(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        float4 p = _vertices[iid];

        const float size = 0.02;

        float x = p.x + size * lerp(-1, 1, vid == 1) * (vid < 2);
        float y = p.y + size * lerp(-1, 1, vid == 3) * (vid >= 2);
        x = (2 * x - 1) * _ScreenParams.y / _ScreenParams.x;
        y =  2 * y - 1;

        float score = p.w;

        v2f o;
        o.position = float4(x, y, 0, 1);
        o.color = float4(1, 0, 0, score);
        return o;
    }

    float4 Fragment(v2f i): SV_Target
    {
        return i.color;
    }

    ENDCG

    SubShader
    {
        ZWrite Off ZTest Always Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}