using System;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// Provides methods for managing the user's authentication and session.
    /// </summary>
    public static class AuthManager
    {
        private static readonly AuthenticationRequests AuthenticationRequests;
        private static UserSession userSession;
        public static UserSession UserSession => userSession;

        public static bool IsSignedIn;

        public static Action<UserSession> SignedIn;

        static AuthManager()
        {
            AuthenticationRequests = new AuthenticationRequests(CoreSettingsHandler.CoreSettings.Subdomain);
        }

        public static async Task LoginAsAnonymous()
        {
            userSession = await AuthenticationRequests.LoginAsAnonymous();
        }

        public static void SetUser(UserSession session)
        {
            userSession = session;
            IsSignedIn = true;
            SignedIn?.Invoke(userSession);
        }

        public static async void SendEmailCode(string email)
        {
            await AuthenticationRequests.SendCodeToEmail(email);
        }

        public static async Task LoginWithCode(string otp)
        {
            userSession = await AuthenticationRequests.LoginWithCode(otp);
            IsSignedIn = true;
            SignedIn?.Invoke(userSession);
        }

        public static async Task RefreshToken()
        {
            var newTokens = await AuthenticationRequests.RefreshToken(userSession.Token, userSession.RefreshToken);
            userSession.Token = newTokens.Item1;
            userSession.RefreshToken = newTokens.Item2;
        }
    }
}
