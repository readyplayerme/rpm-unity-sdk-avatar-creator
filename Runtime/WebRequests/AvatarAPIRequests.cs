using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AvatarAPIRequests
    {
        private const string PREVIEW_PARAMETER = "preview=true";
        private const string RESPONSE_TYPE_PARAMETER = "responseType=glb";
        private const string COLOR_PARAMETERS = "colors?type=skin,beard,hair,eyebrow";
        private const string FETCH_AVATAR_PARAMETERS = "?select=id,partner&userId=";

        private readonly AuthorizedRequest authorizedRequest;
        private readonly CancellationToken ctx;
        
        public AvatarAPIRequests(CancellationToken ctx = default)
        {
            this.ctx = ctx;
            authorizedRequest = new AuthorizedRequest();
        }

        public async Task<Dictionary<string, string>> FetchUserAvatars(string userId)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V1}{FETCH_AVATAR_PARAMETERS}{userId}",
                    Method = Method.GET,
                },
                ctx: ctx
            );
            
            response.ThrowIfError();

            var json = JObject.Parse(response.Text);
            var data = json["data"]!;
            return data.ToDictionary(element => element["id"]!.ToString(), element => element["partner"]!.ToString());
        }

        public async Task<ColorPalette[]> GetAllAvatarColors(string avatarId)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}/{COLOR_PARAMETERS}",
                    Method = Method.GET,
                },
                ctx: ctx
            );

            response.ThrowIfError();
            return ColorResponseHandler.GetColorsFromResponse(response.Text);
        }

        public async Task<AvatarProperties> GetAvatarMetadata(string avatarId)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}.json",
                    Method = Method.GET,
                },
                ctx: ctx
            );

            response.ThrowIfError();
            
            var json = JObject.Parse(response.Text);
            var data = json["data"]!.ToString();
            return JsonConvert.DeserializeObject<AvatarProperties>(data);
        }

        public async Task<string> CreateNewAvatar(AvatarProperties avatarProperties)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = Endpoints.AVATAR_API_V2,
                    Method = Method.POST,
                    Payload = avatarProperties.ToJson()
                },
                ctx: ctx
            );

            response.ThrowIfError();

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?{PREVIEW_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}&{PREVIEW_PARAMETER}";

            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = url,
                    Method = Method.GET,
                },
                ctx: ctx);


            response.ThrowIfError();
            return response.Data;
        }

        public async Task<byte[]> GetAvatar(string avatarId, string parameters = null)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}",
                    Method = Method.GET,
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}?{RESPONSE_TYPE_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}{parameters}&{RESPONSE_TYPE_PARAMETER}";

            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = url,
                    Method = Method.PATCH,
                    Payload = avatarProperties.ToJson(true)
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                    Method = Method.PUT,
                },
                ctx: ctx);

            response.ThrowIfError();
            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId)
        {
            var response = await authorizedRequest.SendRequest(
                new RequestData
                {
                    Url = $"{Endpoints.AVATAR_API_V1}/{avatarId}",
                    Method = Method.DELETE,
                },
                ctx: ctx);

            response.ThrowIfError();
        }
    }
}
