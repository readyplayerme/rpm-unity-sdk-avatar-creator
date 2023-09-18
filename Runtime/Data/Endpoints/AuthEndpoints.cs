namespace ReadyPlayerMe.AvatarCreator
{
    public abstract class AuthEndpoints : Endpoints
    {
        public static string GetAuthAnonymousEndpoint(string subdomain)
        {
            return string.Format(API_SUBDOMAIN_ENDPOINT, subdomain, "/users");
        }

        public static string GetAuthStartEndpoint(string subdomain)
        {
            return string.Format(API_SUBDOMAIN_ENDPOINT, subdomain, "/auth/start");
        }

        public static string GetConfirmCodeEndpoint(string subdomain)
        {
            return string.Format(API_SUBDOMAIN_ENDPOINT, subdomain, "/auth/login");
        }

        public static string GetTokenRefreshEndpoint(string subdomain)
        {
            return string.Format(API_SUBDOMAIN_ENDPOINT, subdomain, "/auth/refresh");
        }
    }
}
