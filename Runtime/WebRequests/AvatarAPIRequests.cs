using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AvatarCreator
{
    public class AvatarAPIRequests
    {
        private const string PREVIEW_PARAMETER = "preview=true";
        private const string RESPONSE_TYPE_PARAMETER = "responseType=glb";

        private readonly Dictionary<string, string> header;
        private readonly CancellationToken cancellationToken;

        public AvatarAPIRequests(string token, CancellationToken cancellationToken = default)
        {
            this.cancellationToken = cancellationToken;
            header = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {token}" }
            };
        }

        public async Task<string> Create(AvatarProperties avatarProperties)
        {
            var response = await WebRequestDispatcher.SendRequest(
                Endpoints.AVATAR_API_V2,
                Method.POST,
                header,
                avatarProperties.ToJson(),
                token: cancellationToken);

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?{PREVIEW_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb{parameters}&{PREVIEW_PARAMETER}";

            var response = await WebRequestDispatcher.SendRequest(
                url,
                Method.GET,
                header,
                token: cancellationToken);
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties, string parameters = null)
        {
            var url = string.IsNullOrEmpty(parameters)
                ? $"{Endpoints.AVATAR_API_V2}/{avatarId}?{RESPONSE_TYPE_PARAMETER}"
                : $"{Endpoints.AVATAR_API_V2}/{avatarId}{parameters}&{RESPONSE_TYPE_PARAMETER}";

            var response = await WebRequestDispatcher.SendRequest(
                url,
                Method.PATCH,
                header,
                avatarProperties.ToJson(true),
                token: cancellationToken);

            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                Method.PUT,
                header,
                token: cancellationToken);

            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId)
        {
            await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V1}/{avatarId}",
                Method.DELETE,
                header,
                token: cancellationToken);
        }
    }
}
