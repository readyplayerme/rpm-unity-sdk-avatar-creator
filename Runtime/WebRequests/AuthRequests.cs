using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AuthRequests
    {
        private readonly string domain;

        public AuthRequests(string domain)
        {
            this.domain = domain;
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            var url = Endpoints.AUTH_USERS.Replace("[domain]", domain);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
            };

            var request = await WebRequestDispatcher.SendRequest(url, Method.POST, headers);

            if (string.IsNullOrEmpty(request.Text))
            {
                throw new Exception("No response received");
            }

            var response = JObject.Parse(request.Text);
            var data = response.GetValue("data");

            if (data == null)
            {
                throw new Exception("No data received");
            }

            return JsonConvert.DeserializeObject<UserSession>(data!.ToString());
        }

        public async Task SendEmailOTP(string email)
        {
            var url = Endpoints.AUTH_START.Replace("[domain]", domain);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
            };

            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("email", email),
                    new JProperty("authType", "code")
                )
            ));

            await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
        }

        public async Task LoginWithOTP(string otp)
        {
            var url = Endpoints.AUTH_LOGIN.Replace("[domain]", domain);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
            };

            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("code", otp)
                )
            ));

            var response = await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
            var json = JObject.Parse(response.Text);
            var data = json.GetValue("data");

            if (data == null)
            {
                throw new Exception("No data received");
            }

        }

        public async Task RefreshToken(string token, string refreshToken)
        {
            var url = Endpoints.AUTH_REFRESH.Replace("[domain]", domain);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
            };

            var payload = new JObject(new JProperty("data",
                new JObject(
                    new JProperty("token", token),
                    new JProperty("refreshToken", refreshToken)
                )
            ));

            var response = await WebRequestDispatcher.SendRequest(url, Method.POST, headers, payload.ToString());
            var json = JObject.Parse(response.Text);
            var data = json.GetValue("data");

            if (data == null)
            {
                throw new Exception("No data received");
            }

            Debug.Log(response.Text);
        }

        private void GetUrl(string domain)
        {
            
        }
    }
}
