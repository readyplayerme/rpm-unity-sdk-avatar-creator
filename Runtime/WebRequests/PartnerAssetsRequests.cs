using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public class PartnerAssetsRequests
    {
        private const int LIMIT = 100;

        private readonly string token;

        private readonly AuthorizedRequest authorizedRequest;
        private string appId = "645b4dd53aef3a0696a2b32c";

        private readonly Dictionary<string, Texture> icons;

        public PartnerAssetsRequests()
        {
            authorizedRequest = new AuthorizedRequest();
            icons = new Dictionary<string, Texture>();
        }

        public async Task<PartnerAsset[]> Get(AssetType assetType, CancellationToken ctx = new CancellationToken())
        {
            var assets = new HashSet<PartnerAsset>();
            var assetData = await GetRequest(LIMIT, 1, assetType, ctx: ctx);
            assets.UnionWith(assetData.Assets);

            for (int i = 2; i <= assetData.Pagination.TotalPages; i++)
            {
                assetData = await GetRequest(LIMIT, i, assetType, ctx: ctx);
                assets.UnionWith(assetData.Assets);
            }

            return assets.ToArray();
        }

        private async Task<AssetData> GetRequest(int limit, int pageNumber, AssetType? assetType = null, CancellationToken ctx = new CancellationToken())
        {
            var url = $"{Endpoints.ASSET_API_V2}?filter=viewable-by-user-and-app&filterUserId={AuthManager.UserSession.Id}&filterApplicationId={appId}";
            url += $"&limit={limit}&page={pageNumber}";

            if (assetType != null)
            {
                var type = AssetTypeHelper.PartnerAssetTypeMap.First(x => x.Value == assetType).Key;
                url += $"&type={type}";
            }
            Debug.Log($"url: {url}");
            var response = await authorizedRequest.SendRequest<Response>(new RequestData
            {
                Url = url,
                Method = HttpMethod.GET
            }, ctx);
            response.ThrowIfError();

            JObject json = JObject.Parse(response.Text);
            PartnerAsset[] partnerAssets = JsonConvert.DeserializeObject<PartnerAsset[]>(json["data"]!.ToString());
            var pagination = JsonConvert.DeserializeObject<Pagination>(json["pagination"]!.ToString());
            Debug.Log($"Deserialized");
            return new AssetData
            {
                Assets = partnerAssets,
                Pagination = pagination
            };
        }

        public async Task<Texture> GetAssetIcon(string url, Action<Texture> completed, CancellationToken ctx = new CancellationToken())
        {
            if (icons.ContainsKey(url))
            {
                completed?.Invoke(icons[url]);
                return icons[url];
            }

            var downloadHandler = new DownloadHandlerTexture();
            var response = await authorizedRequest.SendRequest<ResponseTexture>(new RequestData
            {
                Url = url,
                Method = HttpMethod.GET,
                DownloadHandler = downloadHandler
            }, ctx: ctx);

            response.ThrowIfError();
            
            icons.Add(url, response.Texture);
            completed?.Invoke(response.Texture);
            return response.Texture;
        }
    }
}
