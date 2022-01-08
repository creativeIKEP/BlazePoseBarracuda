#ifndef BLAZEPOSE_COLORSPACE
#define BLAZEPOSE_COLORSPACE


#define FLT_EPSILON 1.192092896e-07

float3 PositivePow(float3 base, float3 power)
{
    return pow(max(abs(base), float3(FLT_EPSILON, FLT_EPSILON, FLT_EPSILON)), power);
}

half3 LinearToSRGB(half3 c)
{
    half3 sRGBLo = c * 12.92;
    half3 sRGBHi = (PositivePow(c, half3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
    half3 sRGB = (c <= 0.0031308) ? sRGBLo : sRGBHi;
    return sRGB;
}

// reference: https://light11.hatenadiary.com/entry/2020/02/25/204432

#endif