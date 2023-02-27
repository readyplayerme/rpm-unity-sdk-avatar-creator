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
        private GameObject avatar;

        [SetUp]
        public void Setup()
        {
            authManager = new AuthManager(DOMAIN);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(avatar);
        }

        [Test]
        public async Task Receive_Partner_Assets()
        {
            var userStore = await authManager.Login();
            var partnerAssetManager = new PartnerAssetsManager(userStore.Token, DOMAIN, BodyType.FullBody, OutfitGender.Masculine);
            var avatarAssets = await partnerAssetManager.GetAllAssets();

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
            avatar = await avatarManager.Create(avatarProperties);

            Assert.IsNotNull(avatar);
            Debug.Log("Avatar created with id: " + avatar.name);

            avatar = await avatarManager.Update(2.ToString(), AssetType.SkinColor);
            Assert.IsNotNull(avatar);
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
    }
}
