using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AuthenticationRequests
    {
        private readonly string domain;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
        };

        private readonly WebRequestDispatcher webRequestDispatcher;

        public AuthenticationRequests(string domain)
        {
            this.domain = domain;
            webRequestDispatcher = new WebRequestDispatcher();
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            var url = GetUrl(Endpoints.AUTH_USERS);

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers);
            response.ThrowIfError();

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
            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload.ToString());
            response.ThrowIfError();
        }

        public async Task<UserSession> LoginWithCode(string code)
        {
            var url = GetUrl(Endpoints.AUTH_LOGIN);
            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("code", code)
                )
            ));

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload.ToString());
            response.ThrowIfError();

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

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload.ToString());
            response.ThrowIfError();

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
