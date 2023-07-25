using System.Threading.Tasks;
using NUnit.Framework;
using ReadyPlayerMe.Core;
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
    }
}
