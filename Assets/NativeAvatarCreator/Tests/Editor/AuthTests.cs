using System.Threading.Tasks;
using NativeAvatarCreator;
using NUnit.Framework;

namespace Tests
{
    public class AuthTests
    {
        [Test]
        public async Task Login_As_Anonymous()
        {
            var userStore = await AuthRequests.LoginAsAnonymous("dev-sdk");

            Assert.False(string.IsNullOrEmpty(userStore.Id));
            Assert.False(string.IsNullOrEmpty(userStore.Token));
        }
    }
}
