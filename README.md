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
- [Ready Player Me Core](https://github.com/readyplayerme/rpm-unity-sdk-core) -  v3.2.0+  
- [glTFast](https://github.com/atteneder/glTFast) - v5.0.0

## Supported Platforms
- Windows/Mac/Linux Standalone
- Android*
- iOS*

## Quick Start

The installation steps can be found [here.](Documentation~/QuickStart.md)

## Important

- The plugin is currently in **beta** stage. We recommend not to use it in the production until the stable version is released.
- AvatarCreator requires the App Id property to be set. Make sure that you set the AppId of your application in the Ready Player Me > Settings > App Id. You can find the AppId of your application in the Studio.
- **Important! AppID must belong to the subdomain you set otherwise authorization will fail.** Login to [studio.readyplayer.me](https://studio.readyplayer.me/applications) to check your subdomain and AppID.

## Features
- Avatar creation through image.
- API for fetching the default avatars.
- API for login with email and a verification code.
- API for fetching the user avatars.
- API for color selection.
- UI for the new APIs in the sample.

## Structure
- The package contains APIs required for creating, customizing and loading the avatar. 
- It also contains a sample which demonstrate the usage of the APIs and replicates RPM web avatar creator.
- The documentation of provided sample can be found [here.](Documentation~/SampleStructure.md)

## Customization

The documentation for customization can be found [here.](Documentation~/CustomizationGuide.md)

## Note

- [*]Camera support is currently only available for PC using Unity’s webcam native API.
- Unity does not have a native file picker, so we have discontinued support for this feature.
- To add support for camera on other platforms and file picker, you will need to implement it yourself.
