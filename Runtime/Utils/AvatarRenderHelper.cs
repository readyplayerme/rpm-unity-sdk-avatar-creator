using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarRenderHelper
    {
        public static async Task<Texture2D> GetPortrait(string avatarId)
        {
            var webRequestDispatcher = new WebRequestDispatcher();
            var response = await webRequestDispatcher.SendRequest<ResponseTexture>($"{Endpoints.AVATAR_API_V1}/{avatarId}.png", HttpMethod.GET,
                downloadHandler: new DownloadHandlerTexture());
            response.ThrowIfError();
            return response.Texture;
        }
    }
}
