### [1.3.0] - 2022-09-25
Improved performance difference by FPS.

### [1.2.0] - 2021-02-15
- Improve the stability of pose estimation.
- Fixed an issue that pose estimation was not performed correctly when the color space was Liner.
- Add new methods (`GetPoseLandmark` and `GetPoseWorldLandmark`) for accessing data with CPU (C#).
- Automatically load `BlazePoseResource` asset data. The constructor arguments are not required.
- The `BlazePoseModel` argument is now optional. The only argument in the inference is the `Texture` data.

### [1.1.1] - 2021-10-04
Change the install method from GitHub URL to [npmjs.com](https://www.npmjs.com/).
Source code change is nothing.

### [1.1.0] - 2021-07-21
Support 3D world landmarks of MediaPipe Pose v0.8.6

### [1.0.0] - 2021-05-30
This is the first release of `creativeIKEP/BlazePoseBarracuda`(`jp.ikep.mediapipe.blazepose`).
