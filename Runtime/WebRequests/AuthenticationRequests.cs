using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AuthenticationRequests
    {
        private readonly string domain;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
        };

        public AuthenticationRequests(string domain)
        {
            this.domain = domain;
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            var url = GetUrl(Endpoints.AUTH_USERS);

            var response = await WebRequestDispatcher.SendRequest(url, Method.POST, headers);

            if (string.IsNullOrEmpty(response.Text))
            {
                throw new Exception("No response received");
            }

            var data = ParseResponse(response.Text);
            return JsonConvert.DeserializeObject<UserSession>(data!.ToString());
        }

        public async Task SendCodeToEmail(string email)
        {
            var url = GetUrl(Endpoints.AUTH_START);
            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("email", email),
                    new JProperty("authType", "code")
                )
            ));

            await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
        }

        public async Task<UserSession> LoginWithCode(string code)
        {
            var url = GetUrl(Endpoints.AUTH_LOGIN);
            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("code", code)
                )
            ));

            var response = await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
            var data = ParseResponse(response.Text);
            return JsonConvert.DeserializeObject<UserSession>(data!.ToString());
        }

        public async Task<(string,string)> RefreshToken(string token, string refreshToken)
        {
            var url = GetUrl(Endpoints.AUTH_REFRESH);
            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("token", token),
                    new JProperty("refreshToken", refreshToken)
                )
            ));

            var response = await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
            var data = ParseResponse(response.Text);
            var newToken = data["token"]!.ToString();
            var newRefreshToken = data["refreshToken"]!.ToString();
            return (newToken, newRefreshToken);
        }

        private string GetUrl(string endpoint)
        {
            return endpoint.Replace("[domain]", domain);
        }

        private JToken ParseResponse(string response)
        {
            var json = JObject.Parse(response);
            var data = json.GetValue("data");

            if (data == null)
            {
                throw new Exception("No data received");
            }

            return data;
        }
    }
}
