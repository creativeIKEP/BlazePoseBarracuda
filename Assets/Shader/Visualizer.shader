Shader "Hidden/BlazePose/Visualizer"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    // Pose landmarks
    StructuredBuffer<float4> _vertices;
    // Pose world landmarks
    StructuredBuffer<float4> _worldVertices;
    // Count of pose landmarks
    uint _keypointCount;
    float _humanExistThreshold;
    // UI size of input image.
    float2 _uiScale;
    // Index pair of body topology lines. Use x, y only.
    float4 _linePair[35];
    float4x4 _invViewMatrix;

    struct v2f{
        float4 position: SV_POSITION;
        float4 color: COLOR;
    };

    // Draw line by long and narrow quad.
    v2f VertexLine(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {
        // Get 2 points of draw target line.
        uint2 pairIndex = (uint2)_linePair[iid].xy;
        float4 p0 = _vertices[pairIndex[0]];
        float4 p1 = _vertices[pairIndex[1]];

        float2 p0_p1 = p1.xy - p0.xy;
        // Orthogonal vector
        float2 orthogonal = float2(-p0_p1.y, p0_p1.x);
        float len = length(p0_p1);
        // Line thickness
        const float size = 0.005;
        
        // Calculate position of p0 -> p1 vector direction.
        float2 p = p0.xy + len * lerp(0, 1, vid & 1) * normalize(p0_p1);
        // Add position of orthogonal vector direction.
        p += size * lerp(-0.5, 0.5, vid < 2 || vid == 5) * normalize(orthogonal);
        // Map to clip position using ratio of render target size and input image UI.
        p = (2 * p - 1) * _uiScale / _ScreenParams.xy;

        float score = lerp(p0.w, p1.w, vid & 1);
        // Get human visiblity score.
        float isHumanExist = _vertices[_keypointCount].x;

        v2f o;
        o.position = float4(p, 0, 1);
        // Color alpha is defined by landmark visibility.
        // Line color is blue if human do not exist.
        o.color = (isHumanExist >= _humanExistThreshold) ? float4(0, 1, 0, score) : float4(0, 0, 1, score);
        return o;
    }

    // Draw point by quad.
    v2f VertexPoint(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        float4 p = _vertices[iid];
        float score = p.w;
        const float size = 0.01;

        float x = p.x + size * lerp(-0.5, 0.5, vid & 1);
        float y = p.y + size * lerp(-0.5, 0.5, vid < 2 || vid == 5);

        // Map to clip position using ratio of render target size and input image UI.
        x = (2 * x - 1) * _uiScale.x / _ScreenParams.x;
        y = (2 * y - 1) * _uiScale.y / _ScreenParams.y;

        v2f o;
        o.position = float4(x, y, 0, 1);
        // Color alpha is defined by landmark visibility.
        o.color = float4(1, 0, 0, score);
        return o;
    }

    // Draw 3D body topology.
    v2f Vertex3DLine(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        // Get 2 points of draw target line.
        uint2 pairIndex = (uint2)_linePair[iid].xy;
        float4 p0 = _worldVertices[pairIndex[0]];
        float4 p1 = _worldVertices[pairIndex[1]];

        float3 camPos = _WorldSpaceCameraPos;
        float3 dir = p1.xyz - p0.xyz;
		float3 toCamDir = normalize(camPos - p0);
		float3 sideDir = normalize(cross(toCamDir, dir));

        float len = length(dir);
        // Line thickness
        const float size = 0.01;
        // Calculate position of p0 -> p1 vector direction.
        float3 p = p0.xyz + len * lerp(0, 1, vid & 1) * normalize(dir);
        // Add position of orthogonal vector direction.
        p += size * lerp(-0.5, 0.5, vid < 2 || vid == 5) * normalize(sideDir);
        // p = UnityWorldToClipPos(p);

        float score = lerp(p0.w, p1.w, vid & 1);
        // Get human visiblity score.
        float isHumanExist = _worldVertices[_keypointCount].x;

        v2f o;
        o.position = UnityWorldToClipPos(p);
        // Color alpha is defined by landmark visibility.
        // Line color is blue if human do not exist.
        o.color = (isHumanExist >= _humanExistThreshold) ? float4(0, 1, 0, score) : float4(0, 0, 1, score);
        return o;
    }

    // Draw world landmark point by quad.
    v2f Vertex3DPoint(uint vid : SV_VertexID, uint iid : SV_InstanceID)
    {   
        float3 p = _worldVertices[iid].xyz;
        float score = _worldVertices[iid].w;
        const float size = 0.03;
        
        float x = size * lerp(-0.5, 0.5, vid & 1);
        float y = size * lerp(-0.5, 0.5, vid < 2 || vid == 5);
        p += mul(_invViewMatrix, float3(x, y, 0));

        v2f o;
        o.position = UnityObjectToClipPos(float4(p, 1));
        // Color alpha is defined by landmark visibility.
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
            #pragma vertex VertexLine
            #pragma fragment Fragment
            ENDCG
        }
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
            #pragma vertex Vertex3DLine
            #pragma fragment Fragment
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex3DPoint
            #pragma fragment Fragment
            ENDCG
        }
    }
}