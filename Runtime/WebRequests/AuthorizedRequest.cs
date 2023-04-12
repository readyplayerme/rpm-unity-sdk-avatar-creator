using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe.AvatarCreator
{
    public struct RequestData
    {
        public string Url;
        public Method Method;
        public string Payload;
        public DownloadHandler DownloadHandler;
    }

    public class AuthorizedRequest
    {
        public async Task<Response> SendRequest(RequestData requestData, CancellationToken ctx = new CancellationToken())
        {
            var response = await Send(requestData, ctx);

            if (response is { IsSuccess: false, ResponseCode: 401 })
            {
                await RefreshTokens();
                response = await Send(requestData, ctx);
            }

            return response;
        }

        private async Task<Response> Send(RequestData requestData, CancellationToken ctx)
        {
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {AuthManager.UserSession.Token}" }
            };

            return await WebRequestDispatcher.SendRequest(
                requestData.Url,
                requestData.Method,
                headers, 
                requestData.Payload,
                requestData.DownloadHandler,
                ctx:ctx
            );
        }

        private async Task RefreshTokens()
        {
            await AuthManager.RefreshToken();
        }
    }
}
