using System.Threading.Tasks;
using NUnit.Framework;

namespace ReadyPlayerMe.AvatarCreator.Tests
{
    public class AuthTests
    {
        private AuthManager authManager;

        [SetUp]
        public void Setup()
        {
            authManager = new AuthManager("dev-sdk");
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
