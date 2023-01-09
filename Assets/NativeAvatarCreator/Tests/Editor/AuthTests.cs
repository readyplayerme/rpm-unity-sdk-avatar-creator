using System.Threading.Tasks;
using NUnit.Framework;

namespace ReadyPlayerMe.NativeAvatarCreator.Tests
{
    public class AuthTests
    {
        [Test]
        public async Task AnonymousAuthTest()
        {
            var userStore = await LoginUtil.LoginAsAnonymous();

            Assert.False(string.IsNullOrEmpty(userStore.Id));
            Assert.False(string.IsNullOrEmpty(userStore.Token));
        }
    }
}
