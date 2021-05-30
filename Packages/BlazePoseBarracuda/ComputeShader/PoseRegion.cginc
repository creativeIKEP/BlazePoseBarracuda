#ifndef BLAZEPOSE_POSEREGION
#define BLAZEPOSE_POSEREGION

struct PoseRegion
{
    // float4(center_x, center_y, size, angle)
    float4 box;
    // delta `box`
    float4 dBox;
    // Image crop matrix
    float4x4 cropMatrix;
};

#endif