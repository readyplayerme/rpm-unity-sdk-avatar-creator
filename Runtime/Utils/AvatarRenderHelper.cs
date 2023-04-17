using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarRenderHelper
    {
        public static async Task<Texture2D> GetPortrait(string avatarId, CancellationToken token = default)
        {
            var webRequestDispatcher = new WebRequestDispatcher();
            var response = await webRequestDispatcher.SendRequest<ResponseTexture>($"{Endpoints.AVATAR_API_V1}/{avatarId}.png", HttpMethod.GET,
                downloadHandler: new DownloadHandlerTexture(), ctx: token);
            response.ThrowIfError();
            return response.Texture;
        }
    }
}
