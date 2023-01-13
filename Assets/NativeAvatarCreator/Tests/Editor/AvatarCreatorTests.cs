using System.Threading.Tasks;
using NativeAvatarCreator;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class AvatarCreatorTests
    {
        private const string DOMAIN = "dev-sdk";

        [Test]
        public async Task Receive_Partner_Assets()
        {
            var userStore = await Auth.LoginAsAnonymous(DOMAIN);
            var avatarAssets = await PartnerAssetsRequests.Get(userStore.Token, DOMAIN);

            Assert.IsNotNull(avatarAssets);
            Assert.Greater(avatarAssets.Length, 0);
        }

        [Test]
        public async Task Avatar_Create_Update_Delete()
        {
            var userStore = await Auth.LoginAsAnonymous(DOMAIN);
            Debug.Log("Logged In with token: " + userStore.Token);

            var avatarCreator = new AvatarAPIRequests(userStore.Token);

            // Create avatar
            var createAvatarPayload = new Payload
            {
                Partner = "dev-sdk",
                Gender = "male",
                BodyType = "fullbody",
                Assets = new PayloadAssets
                {
                    SkinColor = 5,
                    EyeColor = "9781796",
                    HairStyle = "23368535",
                    EyebrowStyle = "41308196",
                    Shirt = "9247449",
                    Outfit = "109373713"
                }
            };

            var avatarId = await avatarCreator.Create(createAvatarPayload);
            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(avatarId);
            Debug.Log("Avatar created with id: " + avatarId);

            // Get Preview GLB
            var previewAvatar = await avatarCreator.GetPreviewAvatar(avatarId);
            Assert.Greater(previewAvatar.Length, 0);
            Debug.Log("Preview avatar download completed.");

            // Update Avatar
            var updateAvatarPayload = new Payload
            {
                Assets = new PayloadAssets
                {
                    SkinColor = 2,
                }
            };
            var updatedAvatar = await avatarCreator.UpdateAvatar(avatarId, updateAvatarPayload);
            Assert.Greater(updatedAvatar.Length, 0);
            Debug.Log("Avatar skinColor updated, and glb downloaded.");

            // Save Avatar
            var metaData = await avatarCreator.SaveAvatar(avatarId);
            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(metaData);
            Debug.Log("Avatar metadata saved to permanent storage on server.");

            // Delete the Avatar
            await avatarCreator.DeleteAvatar(avatarId);
            Debug.Log("Avatar deleted.");
            Assert.Pass();
        }
    }
}
