using System.Threading.Tasks;
using NUnit.Framework;

namespace ReadyPlayerMe.AvatarCreator.Tests
{
    public class AuthTests
    {
        private AuthManager authManager;
        private const string DEV_SDK_DOMAIN = "dev-sdk";

        [SetUp]
        public void Setup()
        {
            authManager = new AuthManager(DEV_SDK_DOMAIN);
        }
        
        [Test]
        public async Task Login_As_Anonymous()
        {
            var userStore = await authManager.Login();

            Assert.False(string.IsNullOrEmpty(userStore.Id));
            Assert.False(string.IsNullOrEmpty(userStore.Token));
        }
    }
}
