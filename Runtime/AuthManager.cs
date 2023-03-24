using System.Threading.Tasks;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// Placeholder for login, signup and signout
    /// </summary>
    public class AuthManager
    {
        private readonly AuthRequests authRequests;
        private UserSession userSession;

        public AuthManager(string domain)
        {
            authRequests = new AuthRequests(domain);
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            return await authRequests.LoginAsAnonymous();
        }

        public async void SendEmailCode(string email)
        {
            await authRequests.SendCodeToEmail(email);
        }

        public async Task<UserSession> LoginWithCode(string otp)
        {
            userSession = await authRequests.LoginWithCode(otp);
            return userSession;
        }

        public async Task RefreshToken(string token, string refreshToken)
        {
            await authRequests.RefreshToken(token, refreshToken);
        }
    }
}
