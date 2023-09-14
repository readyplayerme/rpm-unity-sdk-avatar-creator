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
    public class AvatarAPIRequests
    {
        private const string FULL_BODY = "fullbody";
        private const string HALF_BODY = "halfbody";
        private const string PARTNER = "partner";
        private const string DATA = "data";
        private const string ID = "id";

        private readonly AuthorizedRequest authorizedRequest;
        private readonly CancellationToken ctx;

        public AvatarAPIRequests(CancellationToken ctx = default)
        {
            this.ctx = ctx;
            authorizedRequest = new AuthorizedRequest();
        }

        public async Task<Dictionary<string, string>> GetUserAvatars(string userId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetUserAvatarsEndpoint(userId),
                    Method = HttpMethod.GET
                },
                ctx: ctx
            );
            response.ThrowIfError();

            var json = JObject.Parse(response.Text);
            var data = json[DATA]!;
            return data.ToDictionary(element => element[ID]!.ToString(), element => element[PARTNER]!.ToString());
        }

        public async Task<List<TemplateData>> GetTemplates()
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetAllAvatarTemplatesEndpoint(),
                    Method = HttpMethod.GET
                },
                ctx: ctx
            );
            response.ThrowIfError();

            var json = JObject.Parse(response.Text);
            var data = json[DATA]!;
            return JsonConvert.DeserializeObject<List<TemplateData>>(data.ToString());
        }

        public async Task<Texture> GetTemplateAvatarImage(string url)
        {
            var downloadHandler = new DownloadHandlerTexture();
            var webRequestDispatcher = new WebRequestDispatcher();
            var response = await webRequestDispatcher.SendRequest<ResponseTexture>(url, HttpMethod.GET, downloadHandler: downloadHandler, ctx: ctx);

            response.ThrowIfError();
            return response.Texture;
        }

        public async Task<AvatarProperties> CreateFromTemplateAvatar(string templateId, string partner, BodyType bodyType)
        {
            var payloadData = new Dictionary<string, string>
            {
                { nameof(partner), partner },
                { nameof(bodyType), bodyType == BodyType.FullBody ? FULL_BODY : HALF_BODY }
            };

            var payload = AuthDataConverter.CreatePayload(payloadData);

            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetAvatarTemplatesEndpoint(templateId),
                    Method = HttpMethod.POST,
                    Payload = payload
                },
                ctx: ctx
            );

            response.ThrowIfError();

            var json = JObject.Parse(response.Text);
            var data = json[DATA]!.ToString();
            return JsonConvert.DeserializeObject<AvatarProperties>(data);
        }

        public async Task<ColorPalette[]> GetAllAvatarColors(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetColorEndpoint(avatarId),
                    Method = HttpMethod.GET
                },
                ctx: ctx
            );

            response.ThrowIfError();
            return ColorResponseHandler.GetColorsFromResponse(response.Text);
        }

        public async Task<AvatarProperties> GetAvatarMetadata(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetAvatarMetadataEndpoint(avatarId),
                    Method = HttpMethod.GET
                },
                ctx: ctx
            );

            response.ThrowIfError();

            var json = JObject.Parse(response.Text);
            var data = json[DATA]!.ToString();
            return JsonConvert.DeserializeObject<AvatarProperties>(data);
        }

        public async Task<AvatarProperties> CreateNewAvatar(AvatarProperties avatarProperties)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetCreateEndpoint(),
                    Method = HttpMethod.POST,
                    Payload = avatarProperties.ToJson()
                },
                ctx: ctx
            );
            response.ThrowIfError();

            var metadata = JObject.Parse(response.Text);
            var data = metadata[DATA]!.ToString();
            return JsonConvert.DeserializeObject<AvatarProperties>(data);
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId, string parameters = null)
        {
            var url = Endpoints.GetAvatarModelEndpoint(avatarId,true, parameters);

            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = url,
                    Method = HttpMethod.GET
                },
                ctx: ctx);


            response.ThrowIfError();
            return response.Data;
        }

        public async Task<byte[]> GetAvatar(string avatarId, string parameters = null)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url =Endpoints.GetAvatarModelEndpoint(avatarId, false, parameters),
                    Method = HttpMethod.GET
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties, string parameters = null)
        {
            var url = Endpoints.GetUpdateAvatarEndpoint(avatarId, parameters);

            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = url,
                    Method = HttpMethod.PATCH,
                    Payload = avatarProperties.ToJson(true)
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetSaveAvatarEndpoint(avatarId),
                    Method = HttpMethod.PUT
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId, bool isDraft = false)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = Endpoints.GetDeleteAvatarEndpoint(avatarId, isDraft),
                    Method = HttpMethod.DELETE
                },
                ctx: ctx);

            response.ThrowIfError();
        }
    }
}
