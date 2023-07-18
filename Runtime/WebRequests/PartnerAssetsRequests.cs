using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public class PartnerAssetsRequests
    {
        private readonly string token;
        private readonly string domain;

        private readonly AuthorizedRequest authorizedRequest;

        public PartnerAssetsRequests(string domain)
        {
            this.domain = domain;
            authorizedRequest = new AuthorizedRequest();
        }

        public async Task<PartnerAsset[]> Get(CancellationToken ctx = new CancellationToken())
        {
            var response = await authorizedRequest.SendRequest<Response>(new RequestData
            {
                Url = Endpoints.ASSETS.Replace("[domain]", domain),
                Method = HttpMethod.GET
            }, ctx: ctx);

            response.ThrowIfError();
            return JsonConvert.DeserializeObject<PartnerAsset[]>(response.Text);
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
