namespace ReadyPlayerMe.AvatarCreator
{
    public static class Endpoints
    {
        public const string DOMAIN_URL = "https://[domain].readyplayer.me";
        public const string ASSETS = DOMAIN_URL + "/api/assets";

        public const string AUTH_USERS = DOMAIN_URL + "/api/users";
        public const string AUTH_START = DOMAIN_URL + "/api/auth/start";
        public const string AUTH_LOGIN = DOMAIN_URL + "/api/auth/login";
        public const string AUTH_REFRESH = DOMAIN_URL + "/api/auth/refresh";

        public const string AVATAR_API_V1 = "https://api.readyplayer.me/v1/avatars";
        public const string AVATAR_API_V2 = "https://api.readyplayer.me/v2/avatars";
    }
}
