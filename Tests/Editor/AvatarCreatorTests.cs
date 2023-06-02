using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator.Tests
{
    public class AvatarCreatorTests
    {
        private const string DOMAIN = "demo";

        private GameObject avatar;

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(avatar);
        }

        [Test]
        public async Task Receive_Partner_Assets()
        {
            await AuthManager.LoginAsAnonymous();
            var partnerAssetManager = new PartnerAssetsManager(DOMAIN, BodyType.FullBody, OutfitGender.Masculine);
            var avatarAssets = await partnerAssetManager.GetAllAssets();

            Assert.IsNotNull(avatarAssets);
            Assert.Greater(avatarAssets.Count, 0);
        }

        [Test]
        public async Task Avatar_Create_Update_Delete()
        {
            await AuthManager.LoginAsAnonymous();
            Debug.Log("Logged In with token: " + AuthManager.UserSession.Token);

            // Create avatar
            var avatarProperties = new AvatarProperties
            {
                Partner = DOMAIN,
                Gender = OutfitGender.Masculine,
                BodyType = BodyType.FullBody,
                Assets = AvatarPropertiesConstants.MaleDefaultAssets
            };

            var avatarManager = new AvatarManager(avatarProperties.BodyType, avatarProperties.Gender);
            avatarProperties = await avatarManager.CreateNewAvatar(avatarProperties);
            avatar = await avatarManager.GetPreviewAvatar(avatarProperties.Id);

            Assert.IsNotNull(avatar);
            Debug.Log("Avatar created with id: " + avatar.name);

            avatar = await avatarManager.UpdateAsset(AssetType.SkinColor, 2.ToString());
            Assert.IsNotNull(avatar);
            Debug.Log("Avatar skinColor updated: " + avatar.name);

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
        public async Task Avatar_Create_Preview_Avatar_Get_Colors()
        {
            await AuthManager.LoginAsAnonymous();
            Debug.Log("Logged In with token: " + AuthManager.UserSession.Token);

            // Create avatar
            var avatarProperties = new AvatarProperties
            {
                Partner = DOMAIN,
                Gender = OutfitGender.Masculine,
                BodyType = BodyType.FullBody,
                Assets = AvatarPropertiesConstants.MaleDefaultAssets
            };

            var avatarManager = new AvatarManager(avatarProperties.BodyType, avatarProperties.Gender);
            avatarProperties = await avatarManager.CreateNewAvatar(avatarProperties);
            avatar = await avatarManager.GetPreviewAvatar(avatarProperties.Id);

            var avatarAPIRequests = new AvatarAPIRequests();
            var colors = await avatarAPIRequests.GetAllAvatarColors(avatarProperties.Id);
            Assert.IsNotNull(colors);
            Assert.Greater(colors.Length, 3);
        }
    }
}
