#ifndef BLAZEPOSE_MISC
#define BLAZEPOSE_MISC

float4x4 makeTranslationMatrix(float2 v)
{
    return float4x4(1, 0, 0, v.x,
                    0, 1, 0, v.y,
                    0, 0, 1,   0,
                    0, 0, 0,   1);
}

float4x4 makeRotationMatrix(float x)
{
    return float4x4(cos(x), -sin(x), 0, 0,
                    sin(x),  cos(x), 0, 0,
                         0,       0, 1, 0,
                         0,       0, 0, 1);
}

float4x4 makeScalingMatrix(float x)
{
    return float4x4(x, 0, 0, 0,
                    0, x, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
}

#endif