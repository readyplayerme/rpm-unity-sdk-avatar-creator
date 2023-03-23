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
            var userStore = await authManager.LoginAsAnonymous();

            Assert.False(string.IsNullOrEmpty(userStore.Id));
            Assert.False(string.IsNullOrEmpty(userStore.Token));
        }
        
        [Test]
        public async Task Send_Email_OTP()
        {
            await authManager.SendEmailOTP("robin@wolf3d.io");
        }
        
        [Test]
        public async Task Login_With_OTP()
        {
            await authManager.LoginWithOTP("24M7RN4V");
        }
        
        [Test]
        public async Task Refresh_Token()
        {
            await authManager.RefreshToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiI2YjM2MWE4NS00ZTg5LTRiZTItOTFkMy1iZjVjNTVhMzNiMjUiLCJpYXQiOjE2Nzk1NzkzMjUsImlzcyI6ImFjbWUuY29tIiwic3ViIjoiNmY5NTUxMjktNmJlNy00M2VlLWI0OTgtMzVjMWYyM2NiMjkzIiwianRpIjoiMzU1NWFjOTEtOTcyNi00ZGQxLWE0N2UtY2ZiMTM2ZDZkNGFmIiwiYXV0aGVudGljYXRpb25UeXBlIjoiUEFTU1dPUkRMRVNTIiwiZW1haWwiOiJyb2JpbkB3b2xmM2QuaW8iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicHJlZmVycmVkX3VzZXJuYW1lIjoicm9iaW5faWdtIiwiYXBwbGljYXRpb25JZCI6IjZiMzYxYTg1LTRlODktNGJlMi05MWQzLWJmNWM1NWEzM2IyNSIsInJvbGVzIjpbInJlZ3VsYXIgdXNlciJdLCJ1c2VySWQiOiI2MzNjMTczMTZmYTg1MTAwMDk0ODNkY2MiLCJhYmlsaXR5IjpbWyJtYW5hZ2UiLCJBdmF0YXIiLHsidXNlcklkIjoiNjMzYzE3MzE2ZmE4NTEwMDA5NDgzZGNjIn1dLFsibWFuYWdlIiwiVXNlciIseyJfaWQiOiI2MzNjMTczMTZmYTg1MTAwMDk0ODNkY2MifV0sWyJyZWFkIiwiUGFydG5lciJdLFsicmVhZCxjcmVhdGUiLCJPcmdhbml6YXRpb24iXSxbInJlYWQiLCJBcHBsaWNhdGlvbiJdLFsicmVhZCx1c2UiLCJBc3NldCIseyJwcmVtaXVtIjp7IiRuZSI6dHJ1ZX19XSxbInJlYWQsdXNlIiwiQXNzZXQiLHsiaWQiOnsiJGluIjpbXX19XSxbInJlYWQsdXNlIiwiQXNzZXQiLHsiZ3JvdXBzLmlkIjp7fX1dXSwiZXhwIjoxNjc5NTgwMjI1fQ.uQgIsqdTCQN4ZORVsAdAhDgYhpmibSMh1We8NUOn85U","y51GmFVL9dGdRq-w655Eq0XJYCDWLajpEr8vBsxoCY0rqVMX0p4aEA");
        }
    }
}
