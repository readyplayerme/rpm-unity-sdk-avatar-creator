using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarRenderHelper
    {
        public static async Task<Texture2D> GetPortrait(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest($"{Endpoints.AVATAR_API_V1}/{avatarId}.png", Method.GET,
                downloadHandler: new DownloadHandlerTexture());
            if (!response.IsSuccess)
            {
                throw new Exception(response.Text);
            }
            
            return (Texture2D)response.Texture;
        }
    }
}
