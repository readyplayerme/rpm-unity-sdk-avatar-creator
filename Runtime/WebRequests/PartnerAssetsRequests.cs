using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AvatarCreator
{
    public static class PartnerAssetsRequests
    {
        public static async Task<PartnerAsset[]> Get(string token, string domain)
        {
            var request = await WebRequestDispatcher.SendRequest(
                Endpoints.ASSETS.Replace("[domain]", domain),
                Method.GET,
                new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                });

            return JsonConvert.DeserializeObject<PartnerAsset[]>(request.Text);
        }

        public static async Task<Texture> GetAssetIcon(string token, string url)
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
                downloadHandler);

            return response.Texture;
        }
    }
}
