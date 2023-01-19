using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NativeAvatarCreator
{
    public static class AvatarAPIRequests
    {
        public static async Task<string> Create(string token, AvatarProperties avatarProperties)
        {
            var response = await WebRequestDispatcher.SendRequest(
                Endpoints.AVATAR_API_V2,
                Method.POST, GetToken(token),
                avatarProperties.ToJson());

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public static async Task<byte[]> GetPreviewAvatar(string token, string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?preview=true",
                Method.GET,
                GetToken(token));
            return response.Data;
        }

        public static async Task<byte[]> UpdateAvatar(string token, string avatarId, AvatarProperties avatarProperties)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}?responseType=glb",
                Method.PATCH,
                GetToken(token),
                avatarProperties.ToJson(true));

            return response.Data;
        }

        public static async Task<string> SaveAvatar(string token, string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                Method.PUT,
                GetToken(token));
            return response.Text;
        }

        public static async Task DeleteAvatar(string token, string avatarId)
        {
            await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V1}/{avatarId}",
                Method.DELETE,
                GetToken(token));
        }

        private static Dictionary<string, string> GetToken(string token)
            => new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {token}" }
            };
    }
}
