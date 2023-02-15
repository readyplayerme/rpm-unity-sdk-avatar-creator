using System.Threading.Tasks;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// Placeholder for login, signup and signout
    /// </summary>
    public class AuthManager
    {
        private readonly string domain;

        public AuthManager(string domain)
        {
            this.domain = domain;
        }

        public async Task<UserSession> Login()
        {
            return await AuthRequests.LoginAsAnonymous(domain);
        }
    }
}
