# Customization Guide

This guide will help you to customize the Ready Player Me avatar creator to fit your needs. The UI is build in mind with separation between logic and UI.

## Requirements
You are allowed to change the entire UI, the only thing that you are required to have in your custom implementation is the Ready Player Me sign-in button and Ready Player Me account-creation UI. This is a legal requirement from Ready Player Me.

## Creating a Custom UI From Scratch

All required APIs are provided in the package. They can be used to create a new custom UI. The package also contains a sample UI that can be used as a reference. The following are the most important APIs that you will required.
- AuthManager - Requests for handling authentication.
- AvatarManager - Requests for creating, updating and deleting avatars.
- PartnerAssetManager - Requests for loading partner assets.

## Customizing the Sample

 The sample comes with a default UI similar to the Web avatar creator. The UI is built using uGUI. All major UI components such as screen, asset buttons, asset type panels are prefabs, and can be edited easily. For detailed description of structure of the sample please see the SampleStructure.md.

### Changing background color
A small demonstration on how to change the background color of different screens. 

https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/assets/1121080/ae412932-1bd9-4d00-b6b5-6525adecf9c7

### Adapting UI according to portrait mode
A small demonstration on how to change avatar creation selection panels according to portrait mode.

https://github.com/readyplayerme/rpm-unity-sdk-avatar-creator/assets/1121080/f706e33d-8fb8-4226-8d3c-3e5b6bb17026




