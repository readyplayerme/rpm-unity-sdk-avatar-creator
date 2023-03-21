using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class PartnerAssetsRequests
    {
        public static async Task<PartnerAsset[]> Get(string token, string domain, CancellationToken ctx = new CancellationToken())
        {
            var request = await WebRequestDispatcher.SendRequest(
                Endpoints.ASSETS.Replace("[domain]", domain),
                Method.GET,
                new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                }, 
                ctx: ctx);

            return JsonConvert.DeserializeObject<PartnerAsset[]>(request.Text);
        }

        public static async Task<Texture> GetAssetIcon(string token, string url, CancellationToken ctx = new CancellationToken())
        {
            var downloadHandler = new DownloadHandlerTexture();
            var response = await WebRequestDispatcher.SendRequest(
                url,
                Method.GET,
                new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },
                null,
                downloadHandler, 
                ctx);

            return response.Texture;
        }
    }
}
