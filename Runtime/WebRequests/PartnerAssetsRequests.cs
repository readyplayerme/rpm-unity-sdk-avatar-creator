using System.Collections.Generic;
using System.IO;
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
        private readonly string token;

        private readonly AuthorizedRequest authorizedRequest;
        private string appId = "645b4dd53aef3a0696a2b32c";
        
        public PartnerAssetsRequests()
        {
            authorizedRequest = new AuthorizedRequest();
        }

        public async Task<PartnerAsset[]> Get(CancellationToken ctx = new CancellationToken())
        {
            var assets = new HashSet<PartnerAsset>();
            var assetData = await GetRequest(100,1, ctx);
            
            assets.UnionWith(assetData.Assets);

            for (int i = 2; i <= assetData.Pagination.TotalPages; i++)
            {
                assetData =  await GetRequest(100,i, ctx);
                assets.UnionWith(assetData.Assets);
            }

            return assets.ToArray();
        }

        public async Task<AssetData> GetRequest(int limit,int pageNumber, CancellationToken ctx)
        {
            var url = $"{Endpoints.ASSET_API_V2}?limit={limit}&page={pageNumber}&filter=viewable-by-user-and-app&filterUserId={AuthManager.UserSession.Id}&filterApplicationId={appId}";
            var response = await authorizedRequest.SendRequest<Response>(new RequestData
            {
                Url = url,
                Method = HttpMethod.GET
            }, ctx: ctx);
            response.ThrowIfError();
            
            var json = JObject.Parse(response.Text);
            var partnerAssets = JsonConvert.DeserializeObject<PartnerAsset[]>(json["data"]!.ToString());
            var pagination = JsonConvert.DeserializeObject<Pagination>(json["pagination"]!.ToString());
            
            return new AssetData()
            {
                Assets = partnerAssets,
                Pagination = pagination
            };
        }

        public async Task<Texture> GetAssetIcon(string url, CancellationToken ctx = new CancellationToken())
        {
            var downloadHandler = new DownloadHandlerTexture();
            var response = await authorizedRequest.SendRequest<ResponseTexture>(new RequestData
            {
                Url = url,
                Method = HttpMethod.GET,
                DownloadHandler = downloadHandler
            }, ctx: ctx);

            response.ThrowIfError();
            return response.Texture;
        }
    }
}
