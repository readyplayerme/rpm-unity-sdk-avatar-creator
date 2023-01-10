using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NativeAvatarCreator
{
    public class AvatarAPIRequests
    {
        private readonly Dictionary<string, string> headers;

        public AvatarAPIRequests(string token)
        {
            headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {token}" }
            };
        }

        public async Task<string> Create(Payload payload)
        {
            var response = await WebRequestDispatcher.SendRequest(
                Urls.AVATAR_API_V2,
                Method.POST, headers,
                payload.ToBytes());

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Urls.AVATAR_API_V2}/{avatarId}.glb?preview=true",
                Method.GET,
                headers);
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, Payload payload)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Urls.AVATAR_API_V2}/{avatarId}?responseType=glb",
                Method.PATCH,
                headers,
                payload.ToBytes(true));

            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Urls.AVATAR_API_V2}/{avatarId}",
                Method.PUT,
                headers);
            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId)
        {
            await WebRequestDispatcher.SendRequest(
                $"{Urls.AVATAR_API_V1}/{avatarId}",
                Method.DELETE,
                headers);
        }
    }
}
