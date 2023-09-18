using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AuthenticationRequests
    {
        private readonly string domain;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        };

        private readonly WebRequestDispatcher webRequestDispatcher;

        public AuthenticationRequests(string domain)
        {
            this.domain = domain;
            webRequestDispatcher = new WebRequestDispatcher();
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            var url = AuthEndpoints.GetAuthAnonymousEndpoint(domain);

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers);
            response.ThrowIfError();

            var data = AuthDataConverter.ParseResponse(response.Text);
            return JsonConvert.DeserializeObject<UserSession>(data!.ToString());
        }

        public async Task SendCodeToEmail(string email, string userId = "")
        {
            var url = AuthEndpoints.GetAuthStartEndpoint(domain);
            var data = new Dictionary<string, string>
            {
                { AuthConstants.EMAIL, email },
                { AuthConstants.AUTH_TYPE, AuthConstants.AUTH_TYPE_CODE }
            };

            if (!string.IsNullOrEmpty(userId))
            {
                data.Add(AuthConstants.USER_ID, userId);
            }

            var payload = AuthDataConverter.CreatePayload(data);

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload);
            response.ThrowIfError();
        }

        public async Task<UserSession> LoginWithCode(string code)
        {
            var url = AuthEndpoints.GetConfirmCodeEndpoint(domain);
            var payload = AuthDataConverter.CreatePayload(new Dictionary<string, string>
            {
                { AuthConstants.AUTH_TYPE_CODE, code }
            });

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload);
            response.ThrowIfError();

            var data = AuthDataConverter.ParseResponse(response.Text);
            return JsonConvert.DeserializeObject<UserSession>(data!.ToString());
        }

        public async Task Signup(string email, string userId)
        {
            var url = AuthEndpoints.GetAuthStartEndpoint(domain);
            var data = new Dictionary<string, string>
            {
                { AuthConstants.EMAIL, email },
                { AuthConstants.AUTH_TYPE, AuthConstants.AUTH_TYPE_PASSWORD },
                { AuthConstants.USER_ID, userId }
            };

            var payload = AuthDataConverter.CreatePayload(data);
            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload);
            response.ThrowIfError();
        }

        public async Task<(string, string)> RefreshToken(string token, string refreshToken)
        {
            var url = AuthEndpoints.GetTokenRefreshEndpoint(domain);
            var payload = AuthDataConverter.CreatePayload(new Dictionary<string, string>
            {
                { AuthConstants.TOKEN, token },
                { AuthConstants.REFRESH_TOKEN, refreshToken }
            });

            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload);
            response.ThrowIfError();

            var data = AuthDataConverter.ParseResponse(response.Text);
            var newToken = data[AuthConstants.TOKEN]!.ToString();
            var newRefreshToken = data[AuthConstants.REFRESH_TOKEN]!.ToString();
            return (newToken, newRefreshToken);
        }

    }
}
