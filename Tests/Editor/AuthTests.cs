using System.Threading.Tasks;
using NUnit.Framework;

namespace ReadyPlayerMe.AvatarCreator.Tests
{
    public class AuthTests
    {
        [Test]
        public async Task Login_As_Anonymous()
        {
            await AuthManager.LoginAsAnonymous();
            Assert.False(string.IsNullOrEmpty(AuthManager.UserSession.Id));
            Assert.False(string.IsNullOrEmpty(AuthManager.UserSession.RefreshToken));
        }

        [Test]
        public async Task Refresh()
        {
            // await AuthManager.RefreshToken(
            //     "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiI2YjM2MWE4NS00ZTg5LTRiZTItOTFkMy1iZjVjNTVhMzNiMjUiLCJpYXQiOjE2Nzk5MTE1MzAsImlzcyI6ImFjbWUuY29tIiwic3ViIjoiNmY5NTUxMjktNmJlNy00M2VlLWI0OTgtMzVjMWYyM2NiMjkzIiwianRpIjoiNGM5YmQ3MDUtYjJiMC00ZDY4LWI1Y2YtMzc0NmQ4MDI5Y2Q0IiwiYXV0aGVudGljYXRpb25UeXBlIjoiUEFTU1dPUkRMRVNTIiwiZW1haWwiOiJyb2JpbkB3b2xmM2QuaW8iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicHJlZmVycmVkX3VzZXJuYW1lIjoicm9iaW5faWdtIiwiYXBwbGljYXRpb25JZCI6IjZiMzYxYTg1LTRlODktNGJlMi05MWQzLWJmNWM1NWEzM2IyNSIsInJvbGVzIjpbInJlZ3VsYXIgdXNlciJdLCJ1c2VySWQiOiI2MmEwNmE5ZDFlMjcwZDJkMGIwNjRhYzEiLCJhYmlsaXR5IjpbWyJtYW5hZ2UiLCJBdmF0YXIiLHsidXNlcklkIjoiNjJhMDZhOWQxZTI3MGQyZDBiMDY0YWMxIn1dLFsibWFuYWdlIiwiVXNlciIseyJfaWQiOiI2MmEwNmE5ZDFlMjcwZDJkMGIwNjRhYzEifV0sWyJyZWFkIiwiUGFydG5lciJdLFsicmVhZCxjcmVhdGUiLCJPcmdhbml6YXRpb24iXSxbInJlYWQiLCJBcHBsaWNhdGlvbiJdLFsicmVhZCx1c2UiLCJBc3NldCIseyJwcmVtaXVtIjp7IiRuZSI6dHJ1ZX19XSxbInJlYWQsdXNlIiwiQXNzZXQiLHsiaWQiOnsiJGluIjpbXX19XSxbInJlYWQsdXNlIiwiQXNzZXQiLHsiZ3JvdXBzLmlkIjp7IiRpbiI6W119fV1dLCJleHAiOjE2Nzk5MTI0MzB9.proEMaIpSfXJlAcgByOwoqZpJOTZlOvl1m-BxolqRKg",
            //     "uavfXextKABR6l5DTPUSSF3TbKO2UZro7uRbPFl6M21AA_X80IoCHA");
        }
    }
}
