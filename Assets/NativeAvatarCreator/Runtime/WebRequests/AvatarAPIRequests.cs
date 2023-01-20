using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NativeAvatarCreator
{
    public class AvatarAPIRequests
    {
        private Dictionary<string, string> header;

        public AvatarAPIRequests(string token)
        {
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
                avatarProperties.ToJson());

            var metadata = JObject.Parse(response.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();
            return avatarId;
        }

        public async Task<byte[]> GetPreviewAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}.glb?preview=true",
                Method.GET,
                header);
            return response.Data;
        }

        public async Task<byte[]> UpdateAvatar(string avatarId, AvatarProperties avatarProperties)
        {
            Debug.Log(avatarProperties.ToJson());
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}?responseType=glb",
                Method.PATCH,
                header,
                avatarProperties.ToJson(true));

            return response.Data;
        }

        public async Task<string> SaveAvatar(string avatarId)
        {
            var response = await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V2}/{avatarId}",
                Method.PUT,
                header);

            Debug.Log(response.Text);
            return response.Text;
        }

        public async Task DeleteAvatar(string avatarId)
        {
            await WebRequestDispatcher.SendRequest(
                $"{Endpoints.AVATAR_API_V1}/{avatarId}",
                Method.DELETE,
                header);
        }
    }
}
