# BlazePoseBarracuda
<img src="https://user-images.githubusercontent.com/34697515/153871000-da4d9a9a-db93-49f4-8ebf-28e20fe135fa.gif" width="100%">
<img src="https://user-images.githubusercontent.com/34697515/153871121-eb05554c-0785-4986-b8ba-ea8eb959634b.gif" width="100%">

**BlazePoseBarracuda** is a human 2D/3D pose estimation neural network that works with a monocular color camera.

BlazePoseBarracuda is Unity Package that runs the [Mediapipe Pose(BlazePose)](https://google.github.io/mediapipe/solutions/pose) pipeline on the [Unity](https://unity.com/).

BlazePoseBarracuda implementation is inspired by [HandPoseBarracuda](https://github.com/keijiro/HandPoseBarracuda) and I referenced [his](https://github.com/keijiro) source code.(Thanks, [keijiro](https://github.com/keijiro)!).

## Dependencies
BlazePoseBarracuda uses the following sub packages:
- [PoseDetectionBarracuda](https://github.com/creativeIKEP/PoseDetectionBarracuda)
- [PoseLandmarkBarracuda](https://github.com/creativeIKEP/PoseLandmarkBarracuda)

## Install
BlazePoseBarracuda can be installed from npm or GitHub URL.

### Install from npm (Recommend)
BlazePoseBarracuda can be installed by adding following sections to the manifest file (`Packages/manifest.json`).

To the `scopedRegistries` section:
```
{
  "name": "creativeikep",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.ikep" ]
}
```
To the `dependencies` section:
```
"jp.ikep.mediapipe.blazepose": "1.3.0"
```
Finally, the manifest file looks like below:
```
{
    "scopedRegistries": [
        {
            "name": "creativeikep",
            "url": "https://registry.npmjs.com",
            "scopes": [ "jp.ikep" ]
        }
    ],
    "dependencies": {
        "jp.ikep.mediapipe.blazepose": "1.3.0",
        ...
    }
}
```

### Install from GitHub URL
BlazePoseBarracuda can be installed by adding below URLs from the Unity Package Manager's window
```
https://github.com/creativeIKEP/PoseDetectionBarracuda.git?path=Packages/PoseDetectionBarracuda#v1.0.1
```
```
https://github.com/creativeIKEP/PoseLandmarkBarracuda.git?path=Packages/PoseLandmarkBarracuda#v1.1.1
```
```
https://github.com/creativeIKEP/BlazePoseBarracuda.git?path=Packages/BlazePoseBarracuda#v1.3.0
```
or, appending lines to your manifest file(`Packages/manifest.json`) `dependencies` block.
Example is below.
```
{
  "dependencies": {
    "jp.ikep.mediapipe.posedetection": "https://github.com/creativeIKEP/PoseDetectionBarracuda.git?path=Packages/PoseDetectionBarracuda#v1.0.1",
    "jp.ikep.mediapipe.poselandmark": "https://github.com/creativeIKEP/PoseLandmarkBarracuda.git?path=Packages/PoseLandmarkBarracuda#v1.1.1",
    "jp.ikep.mediapipe.blazepose": "https://github.com/creativeIKEP/BlazePoseBarracuda.git?path=Packages/BlazePoseBarracuda#v1.3.0",
    ...
  }
}
```

## Usage Demo
This repository has the demo for inferencing pose and visualizing landmarks.
Check ["/Assets/Script/PoseVisuallizer.cs"](https://github.com/creativeIKEP/BlazePoseBarracuda/blob/main/Assets/Script/PoseVisuallizer.cs) and ["/Assets/Scenes/2DSampleScene.unity"](https://github.com/creativeIKEP/BlazePoseBarracuda/blob/main/Assets/Scenes/2DSampleScene.unity) for BlazePoseBarracuda usage demo details in the 2D pose estimation.
Check ["/Assets/Script/PoseVisuallizer3D.cs"](https://github.com/creativeIKEP/BlazePoseBarracuda/blob/main/Assets/Script/PoseVisuallizer3D.cs) and ["/Assets/Scenes/3DSampleScene.unity"](https://github.com/creativeIKEP/BlazePoseBarracuda/blob/main/Assets/Scenes/3DSampleScene.unity) for BlazePoseBarracuda usage demo details in the 3D pose estimation.

## Demo Image
Videos for demos was downloaded from [pexels](https://www.pexels.com/ja-jp/).
Downloaded videos URLs are below.

- https://www.pexels.com/ja-jp/video/7559286/
- https://www.pexels.com/ja-jp/video/3048954/

## Author
[IKEP](https://ikep.jp)

## LICENSE
Copyright (c) 2021 IKEP

[Apache-2.0](/LICENSE.md)
