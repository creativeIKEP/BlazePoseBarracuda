# BlazePoseBarracuda
![demo_tabletop](/screenshot/demo_tabletop.gif)
![demo_fitness](/screenshot/demo_fitness.gif)
![demo_dance](/screenshot/demo_dance.gif)

**BlazePoseBarracuda** is a human pose estimation neural network that works with a monocular color camera.

BlazePoseBarracuda is Unity Package that runs the [Mediapipe Pose(BlazePose)](https://google.github.io/mediapipe/solutions/pose) pipeline on the [Unity](https://unity.com/).

BlazePoseBarracuda has 2 estimation modes(`Upper Body Only` and `Full Body`) and, can be switched on the realtime.

BlazePoseBarracuda implementation is inspired by [HandPoseBarracuda](https://github.com/keijiro/HandPoseBarracuda) and I referenced [his](https://github.com/keijiro) source code.(Thanks, [keijiro](https://github.com/keijiro)!).

### Dependencies
BlazePoseBarracuda uses the following sub packages:
- [PoseDetectionBarracuda](https://github.com/creativeIKEP/PoseDetectionBarracuda)
- [PoseLandmarkBarracuda](https://github.com/creativeIKEP/PoseLandmarkBarracuda)

### Install
BlazePoseBarracuda can be installed by adding below URLs from the Unity Package Manager's window
```
https://github.com/creativeIKEP/PoseDetectionBarracuda.git?path=Packages/PoseDetectionBarracuda#v1.0.0
```
```
https://github.com/creativeIKEP/PoseLandmarkBarracuda.git?path=Packages/PoseLandmarkBarracuda#v1.0.1
```
```
https://github.com/creativeIKEP/BlazePoseBarracuda.git?path=Packages/BlazePoseBarracuda#v1.0.0
```
or, appending lines to your manifest file(`Packages/manifest.json`) `dependencies` block.
Example is below.
```
{
  "dependencies": {
    "jp.ikep.mediapipe.posedetection": "https://github.com/creativeIKEP/PoseDetectionBarracuda.git?path=Packages/PoseDetectionBarracuda#v1.0.0",
    "jp.ikep.mediapipe.poselandmark": "https://github.com/creativeIKEP/PoseLandmarkBarracuda.git?path=Packages/PoseLandmarkBarracuda#v1.0.1",
    "jp.ikep.mediapipe.blazepose": "https://github.com/creativeIKEP/BlazePoseBarracuda.git?path=Packages/BlazePoseBarracuda#v1.0.0",
    ...
  }
}
```

### Demo Image
Videos for demo was downloaded from [pixabay](https://pixabay.com).
Downloaded videos URLs are below.

- https://pixabay.com/videos/id-49811.
- https://pixabay.com/videos/id-72464.
- https://pixabay.com/videos/id-21827.

### Author
[IKEP](https://ikep.jp)

### LICENSE
Copyright (c) 2021 IKEP

[Apache-2.0](/LICENSE.md)
