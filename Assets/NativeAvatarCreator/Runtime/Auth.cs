using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NativeAvatarCreator
{
    public static class Auth
    {
        public static async Task<UserStore> LoginAsAnonymous(string domain)
        {
            var url = Urls.AUTH_ENDPOINT.Replace("[domain]", domain);
            var request = await WebRequestDispatcher.SendRequest(url, Method.POST);

            Assert.False(string.IsNullOrEmpty(request.Text));

            var response = JObject.Parse(request.Text);
            var data = response.GetValue("data");

            Assert.IsNotNull(data);

            return JsonConvert.DeserializeObject<UserStore>(data!.ToString());
        }
    }
}
