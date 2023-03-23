using System.Threading.Tasks;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// Placeholder for login, signup and signout
    /// </summary>
    public class AuthManager
    {
        private readonly AuthRequests authRequests;
        
        public AuthManager(string domain)
        {
            authRequests = new AuthRequests(domain);
        }

        public async Task<UserSession> LoginAsAnonymous()
        {
            return await authRequests.LoginAsAnonymous();
        }


        public async Task SendEmailOTP(string email)
        {
            await authRequests.SendEmailOTP( email);
        }

        public async Task LoginWithOTP(string otp)
        {
            await authRequests.LoginWithOTP(otp);
        }

        public async Task RefreshToken(string token, string refreshToken)
        {
            await authRequests.RefreshToken(token, refreshToken);
        }
    }
}
