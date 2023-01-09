using System.Threading.Tasks;
using NativeAvatarCreator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ReadyPlayerMe.NativeAvatarCreator.Tests
{
    public static class LoginUtil
    {
        public static async Task<UserStore> LoginAsAnonymous()
        {
            var request = await WebRequestDispatcher.SendRequest(Urls.AUTH_ENDPOINT, Method.POST);

            Assert.False(string.IsNullOrEmpty(request.Text));

            var response = JObject.Parse(request.Text);
            var data = response.GetValue("data");

            Assert.IsNotNull(data);

            return JsonConvert.DeserializeObject<UserStore>(data!.ToString());
        }
    }
}
