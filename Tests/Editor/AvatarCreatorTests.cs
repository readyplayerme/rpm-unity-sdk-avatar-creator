using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator.Tests
{
    public class AvatarCreatorTests
    {
        private const string DOMAIN = "demo";

        private AuthManager authManager;

        [SetUp]
        public void Setup()
        {
            authManager = new AuthManager(DOMAIN);
        }

        [Test]
        public async Task Receive_Partner_Assets()
        {
            var userStore = await authManager.Login();
            var avatarAssets = await PartnerAssetsManager.GetAllAssets(userStore.Token, DOMAIN, BodyType.FullBody, OutfitGender.Masculine);

            Assert.IsNotNull(avatarAssets);
            Assert.Greater(avatarAssets.Count, 0);
        }

        [Test]
        public async Task Avatar_Create_Update_Delete()
        {
            var userStore = await authManager.Login();
            Debug.Log("Logged In with token: " + userStore.Token);

            // Create avatar
            var avatarProperties = new AvatarProperties
            {
                Partner = DOMAIN,
                Gender = OutfitGender.Masculine,
                BodyType = BodyType.FullBody,
                Assets = AvatarPropertiesConstants.MaleDefaultAssets
            };

            var avatarManager = new AvatarManager(userStore.Token, avatarProperties.BodyType, avatarProperties.Gender);
            var avatar = await avatarManager.Create(avatarProperties);

            Assert.IsNotNull(avatar);
            Debug.Log("Avatar created with id: " + avatar.name);

            var updatedAvatar = await avatarManager.Update(2.ToString(), AssetType.SkinColor);
            Assert.IsNotNull(updatedAvatar);
            Debug.Log("Avatar skinColor updated");

            // Save Avatar
            var avatarId = await avatarManager.Save();
            Assert.IsNotNull(avatarId);
            Debug.Log("Avatar metadata saved to permanent storage on server.");

            // Delete the Avatar
            await avatarManager.Delete();
            Debug.Log("Avatar deleted.");
            Assert.Pass();
        }

        [Test]
        public async Task Avatar_Create_And_Load()
        {
            var userStore = await authManager.Login();
            Debug.Log("Logged In with token: " + userStore.Token);

            // Create avatar
            var createAvatarPayload = new AvatarProperties
            {
                Partner = DOMAIN,
                Gender = OutfitGender.Masculine,
                BodyType = BodyType.FullBody,
                Assets = AvatarPropertiesConstants.MaleDefaultAssets
            };

            var avatarAPIRequests = new AvatarAPIRequests(userStore.Token);

            var avatarId = await avatarAPIRequests.CreateNewAvatar(createAvatarPayload);
            Assert.IsNotNull(avatarId);
            Assert.IsNotEmpty(avatarId);
            Debug.Log("Avatar created with id: " + avatarId);

            // Get Preview GLB
            var previewAvatar = await avatarAPIRequests.GetPreviewAvatar(avatarId);
            Assert.Greater(previewAvatar.Length, 0);
            Debug.Log("Preview avatar download completed.");

            var avatarLoader = new InCreatorAvatarLoader();
            var avatar = await avatarLoader.Load(avatarId, createAvatarPayload.BodyType, createAvatarPayload.Gender, previewAvatar);
            Assert.IsNotNull(avatar);
        }
    }
}
