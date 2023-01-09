using System.Collections.Generic;
using System.Threading.Tasks;
using NativeAvatarCreator;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ReadyPlayerMe.NativeAvatarCreator.Tests
{
    public class AvatarCreatorTests
    {
        [Test]
        public async Task Receive_Correct_Partner_Assets_Json()
        {
            var userStore = await LoginUtil.LoginAsAnonymous();
            var request = await WebRequestDispatcher.SendRequest(Urls.ASSETS_ENDPOINT, Method.GET, new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {userStore.Token}" }
            });
            Assert.IsNotNull(request.Text);
            Assert.IsNotEmpty(request.Text);
        }

        [Test]
        public async Task Avatar_Create_Update_Delete()
        {
            var userStore = await LoginUtil.LoginAsAnonymous();
            Debug.Log("Logged In with token: " + userStore.Token);

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {userStore.Token}" }
            };

            // Create avatar
            var createAvatarPayload = new Payload
            {
                Data = new Data
                {

                    Partner = "dev-sdk",
                    Gender = "male",
                    BodyType = "fullbody",
                    Assets = new Assets
                    {
                        SkinColor = 5,
                        EyeColor = "9781796",
                        HairStyle = "23368535",
                        EyebrowStyle = "41308196",
                        Shirt = "9247449",
                        Outfit = "109373713"
                    }
                }
            };

            var createAvatarResponse =
                await WebRequestDispatcher.SendRequest(Urls.AVATAR_API_V2, Method.POST, headers, createAvatarPayload.ToBytes());

            Assert.False(string.IsNullOrEmpty(createAvatarResponse.Text));

            var metadata = JObject.Parse(createAvatarResponse.Text);
            var avatarId = metadata["data"]?["id"]?.ToString();

            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(avatarId);

            Debug.Log("Avatar created with id: " + avatarId);

            // Get Preview GLB
            var getPreviewAvatar = await WebRequestDispatcher.SendRequest($"{Urls.AVATAR_API_V2}/{avatarId}.glb?preview=true", Method.GET, headers);
            Assert.Greater(getPreviewAvatar.Data.Length, 0);
            Debug.Log("Preview avatar download completed.");

            // Update Avatar
            var updateAvatarPayload = new Payload
            {
                Data = new Data
                {
                    Assets = new Assets
                    {
                        SkinColor = 2,
                    }
                }
            };
            var updateAvatarResponse = await WebRequestDispatcher.SendRequest($"{Urls.AVATAR_API_V2}/{avatarId}?responseType=glb", Method.PATCH,
                headers,
                updateAvatarPayload.ToBytes(true));

            Assert.Greater(updateAvatarResponse.Data.Length, 0);
            Debug.Log("Avatar skinColor updated, and glb downloaded.");

            // Save Avatar
            var saveAvatarResponse = await WebRequestDispatcher.SendRequest($"{Urls.AVATAR_API_V2}/{avatarId}", Method.PUT, headers);
            Assert.IsNotEmpty(saveAvatarResponse.Text);
            Debug.Log("Avatar metadata saved to permanent storage on server.");

            // Get Avatar
            var getAvatarResponse = await WebRequestDispatcher.SendRequest($"{Urls.AVATAR_API_V2}/{avatarId}.glb", Method.GET, headers);
            Assert.Greater(getAvatarResponse.Data.Length, 0);
            Debug.Log("Final avatar downloaded.");

            // Delete the Avatar
            await WebRequestDispatcher.SendRequest($"{Urls.AVATAR_API_V1}/{avatarId}", Method.DELETE, headers);
            Debug.Log("Avatar deleted.");
        }
    }
}
