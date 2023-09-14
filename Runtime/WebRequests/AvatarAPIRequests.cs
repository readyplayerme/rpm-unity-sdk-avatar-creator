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
        private const string PREVIEW_PARAMETER = "preview=true";
        private const string RESPONSE_TYPE_PARAMETER = "responseType=glb";
        private const string COLOR_PARAMETERS = "colors?type=skin,beard,hair,eyebrow";
        private const string FETCH_AVATAR_PARAMETERS = "?select=id,partner&userId=";
        private const string DRAFT_PARAMETER = "draft";
        private const string TEMPLATE = "templates";
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
                    Url = $"{Endpoints.AVATAR_API_V1}{FETCH_AVATAR_PARAMETERS}{userId}",
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
                    Url = $"{Endpoints.AVATAR_API_V2}/{TEMPLATE}",
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

        public async Task<AvatarProperties> CreateFromTemplateAvatar(string avatarId, string partner, BodyType bodyType)
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
                    Url = $"{Endpoints.AVATAR_API_V2}/templates/{avatarId}",
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
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}/{COLOR_PARAMETERS}",
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
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}.json",
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
                    Url = Endpoints.AVATAR_API_V2,
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
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?{PREVIEW_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}&{PREVIEW_PARAMETER}";
            Debug.Log($"PREVIEW URL = {url}");
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

        [System.Serializable]
        public class Payload
        {
            public Dictionary<string, object> Data;
        }

        public async Task PrecompileAvatar(string avatarId, PrecompileData precompileData, string parameters = null)
        {
            var startTime = Time.time;
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}/precompile"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}/precompile{parameters}";
            var json = JsonConvert.SerializeObject(precompileData);

            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = url,
                    Method = HttpMethod.POST,
                    Payload = json
                },
                ctx: ctx);
            Debug.Log($"Response status = {response.IsSuccess} \n Response text = {response.Text}");
            Debug.Log($"PRECOMPILE URL = {url} \n PRECOMPILE PAYLOAD= {json} timeElapsed = {Time.time - startTime}");
            response.ThrowIfError();
        }

        public async Task<byte[]> GetAvatar(string avatarId, string parameters = null)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}",
                    Method = HttpMethod.GET
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties, string parameters = null)
        {
            var startTime = Time.time;
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}?{RESPONSE_TYPE_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}{parameters}&{RESPONSE_TYPE_PARAMETER}";
            Debug.Log($"UpdateAvatar URL = {url}");

            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = url,
                    Method = HttpMethod.PATCH,
                    Payload = avatarProperties.ToJson(true)
                },
                ctx: ctx);

            response.ThrowIfError();
            Debug.Log($"Update URL = {url} \n timeElapsed = {Time.time - startTime}");

            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                    Method = HttpMethod.PUT
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Text;
        }

        public async Task DeleteAvatarDraft(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}/{DRAFT_PARAMETER}",
                    Method = HttpMethod.DELETE
                },
                ctx: ctx);

            response.ThrowIfError();
        }

        public async Task DeleteAvatar(string avatarId)
        {
            var response = await authorizedRequest.SendRequest<Response>(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                    Method = HttpMethod.DELETE
                },
                ctx: ctx);

            response.ThrowIfError();
        }
    }
}
