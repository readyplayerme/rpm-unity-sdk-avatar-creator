﻿using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarRenderHelper
    {
        public static async Task<Texture2D> GetPortrait(string avatarId)
        {
            var isCompleted = false;
            var renderLoader = new AvatarRenderLoader();
            Texture2D image = null;
            renderLoader.OnFailed = (type, s) =>
            {
                Debug.LogError("Failed: ");
            };
            renderLoader.OnCompleted = renderImage =>
            {
                image = renderImage;
                isCompleted = true;
            };
            renderLoader.LoadRender($"{Endpoints.AVATAR_API_V1}/{avatarId}.glb", AvatarRenderScene.Portrait);
            while (!isCompleted)
            {
                await Task.Yield();
            }

            return image;
        }
    }
}