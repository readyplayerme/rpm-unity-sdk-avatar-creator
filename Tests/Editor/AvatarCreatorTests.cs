using System.Collections.Generic;
using System.Threading.Tasks;
using NativeAvatarCreator;
using NUnit.Framework;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace Tests
{
    public class AvatarCreatorTests
    {
        private const string DOMAIN = "dev-sdk";

        [Test]
        public async Task Receive_Partner_Assets()
        {
            var userStore = await AuthRequests.LoginAsAnonymous(DOMAIN);
            var avatarAssets = await PartnerAssetsRequests.Get(userStore.Token, DOMAIN);
            
            Assert.IsNotNull(avatarAssets);
            Assert.Greater(avatarAssets.Length, 0);
        }

        [Test]
        public async Task Avatar_Create_Update_Delete()
        {
            var userStore = await AuthRequests.LoginAsAnonymous(DOMAIN);
            Debug.Log("Logged In with token: " + userStore.Token);

            // Create avatar
            var createAvatarPayload = new AvatarProperties
            {
                Partner = DOMAIN,
                Gender = OutfitGender.Masculine,
                BodyType = BodyType.FullBody,
                Assets = new Dictionary<AssetType, object>()
                {
                    { AssetType.SkinColor, 5 },
                    { AssetType.EyeColor, "9781796" }
                }
            };

            Debug.Log(createAvatarPayload.ToJson());

            var avatarAPIRequests = new AvatarAPIRequests(userStore.Token);

            var avatarId = await avatarAPIRequests.Create(createAvatarPayload);
            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(avatarId);
            Debug.Log("Avatar created with id: " + avatarId);

            // Get Preview GLB
            var previewAvatar = await avatarAPIRequests.GetPreviewAvatar(avatarId);
            Assert.Greater(previewAvatar.Length, 0);
            Debug.Log("Preview avatar download completed.");

            // Update Avatar
            var updateAvatarPayload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
                {
                    { AssetType.SkinColor, 2 }
                }
            };
            var updatedAvatar = await avatarAPIRequests.UpdateAvatar(avatarId, updateAvatarPayload);
            Assert.Greater(updatedAvatar.Length, 0);
            Debug.Log("Avatar skinColor updated, and glb downloaded.");

            // Save Avatar
            var metaData = await avatarAPIRequests.SaveAvatar(avatarId);
            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(metaData);
            Debug.Log("Avatar metadata saved to permanent storage on server.");

            // Delete the Avatar
            await avatarAPIRequests.DeleteAvatar(avatarId);
            Debug.Log("Avatar deleted.");
            Assert.Pass();
        }
    }
}
