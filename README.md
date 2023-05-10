# Ready Player Me Unity SDK Avatar Creator

[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/readyplayerme/rpm-unity-sdk-avatar-creator?include_prereleases)](https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/releases/latest) [![GitHub Discussions](https://img.shields.io/github/discussions/readyplayerme/rpm-unity-sdk-avatar-creator)](https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/discussions)

![image](https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/assets/1121080/ec555611-829a-44b7-b215-10e188a25b85)

**Ready Player Me Avatar Creator** is an extension to www.readyplayer.me avatar platform, which helps you create avatars natively.

Please visit the online documentation and join our public `discord` community.

![](https://i.imgur.com/zGamwPM.png) **[Online Documentation]( https://readyplayer.me/docs )**

![](https://i.imgur.com/FgbNsPN.png) **[Discord Channel]( https://discord.gg/9veRUu2 )**

:octocat: **[GitHub Discussions]( https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/discussions )**

## Requirements
- Unity Version 2020.3 or higher
- [Git](https://git-scm.com) needs to be installed to fetch the Unity package. [Download here](https://git-scm.com/downloads)
- [Ready Player Me Core](https://github.com/readyplayerme/rpm-unity-sdk-core) - v1.2.0
- [Ready Player Me Avatar Loader](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader) - v1.2.0
- [Ready Player Me WebView](https://github.com/readyplayerme/rpm-unity-sdk-webview) - v1.1.0
- [glTFast](https://github.com/atteneder/glTFast) - v5.0.0

## Quick Start

The installation steps can be found [here.](https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/blob/main/Documentation~/QuickStart.md)

## Important

The plugin is currently in **alpha** stage. We recommend not to use it in the production until the stable version is released.

## Features
- Avatar creation through image.
- API for fetching the default avatars.
- API for login with email and a verification code.
- API for fetching the user avatars.
- API for color selection.
- UI for the new APIs in the sample.

## Extra
![image](https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/assets/1121080/f6d6d847-9244-41bc-a73c-770030ed075f)
- **State To Skip**  Allows skipping state, eg. gender selection screen.
- **Default Gender and BodyType** These will be used when gender selection or body type selection are skipped.
- **Partner Domain**  This can be set using rpm settings menu,

## Note

### Camera
Webcam support is currently only available for PC using Unity’s native API.
To add support for other platforms, you will need to implement it yourself.

### File Picker
Unity does not have a native file picker, so we have discontinued support for this feature.
To add support for a file picker, you will need to implement it yourself.
