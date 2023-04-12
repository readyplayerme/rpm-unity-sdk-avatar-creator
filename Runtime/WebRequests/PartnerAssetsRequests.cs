using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            var response = await authorizedRequest.SendRequest(new RequestData
            {
                Url = Endpoints.ASSETS.Replace("[domain]", domain),
                Method = Method.GET,
            }, ctx: ctx);

            if (!response.IsSuccess)
            {
                throw new Exception(response.Text);
            }

            return JsonConvert.DeserializeObject<PartnerAsset[]>(response.Text);
        }

        public async Task<Texture> GetAssetIcon(string url, CancellationToken ctx = new CancellationToken())
        {
            var downloadHandler = new DownloadHandlerTexture();
            var response = await authorizedRequest.SendRequest(new RequestData
                {
                    Url = url,
                    Method = Method.GET,
                    DownloadHandler = downloadHandler,
                }, ctx: ctx);

            if (!response.IsSuccess)
            {
                throw new Exception(response.Text);
            }

            return response.Texture;
        }
    }
}
