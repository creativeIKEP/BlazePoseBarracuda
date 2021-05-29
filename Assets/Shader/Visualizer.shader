Shader "Hidden/BlazePose/Visualizer"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    StructuredBuffer<float4> _vertices;
    float2 _uiScale;
    float4 _linePair[35];

    struct v2f{
        float4 position: SV_POSITION;
        float4 color: COLOR;
    };

    v2f VertexPoint(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        float4 p = _vertices[iid];

        const float size = 0.02;

        float x = p.x + size * lerp(-1, 1, vid == 1) * (vid < 2);
        float y = p.y + size * lerp(-1, 1, vid == 3) * (vid >= 2);
        x = (2 * x - 1) * _uiScale.x / _ScreenParams.x;
        y = (2 * y - 1) *  _uiScale.y / _ScreenParams.y;

        float score = p.w;

        v2f o;
        o.position = float4(x, y, 0, 1);
        o.color = float4(1, 0, 0, score);
        return o;
    }

    v2f VertexLine(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        v2f o;
        uint2 pairIndex = (uint2)_linePair[iid].xy;
        float4 p0 = _vertices[pairIndex[0]];
        float4 p1 = _vertices[pairIndex[1]];
        float x = lerp(p0.x, p1.x, vid);
        float y = lerp(p0.y, p1.y, vid);
        float score = lerp(p0.w, p1.w, vid);

        x = (2 * x - 1) * _uiScale.x / _ScreenParams.x;
        y = (2 * y - 1) *  _uiScale.y / _ScreenParams.y;

        o.position = float4(x, y, 0, 1);
        o.color = float4(0, 1, 0, score);
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
            #pragma vertex VertexPoint
            #pragma fragment Fragment
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex VertexLine
            #pragma fragment Fragment
            ENDCG
        }
    }
}