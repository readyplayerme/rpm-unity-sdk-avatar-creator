namespace ReadyPlayerMe.AvatarCreator
{
    public static class Endpoints
    {
        private const string API_ENDPOINT = "https://{0}.readyplayer.me/api{1}";
        private const string AVATAR_API_V2_ENDPOINT = "https://api.readyplayer.me/v2/avatars";
        private const string AVATAR_API_V1_ENDPOINT = "https://api.readyplayer.me/v1/avatars";
        private const string ASSET_ENDPOINT = "https://api.readyplayer.me/v1/assets?limit={0}&page={1}&filter=viewable-by-user-and-app&filterUserId={2}&filterApplicationId={3}&gender=neutral&gender={4}";
        private const string MODELS_URL_PREFIX = "https://models.readyplayer.me";

        public static string GetAuthAnonymousEndpoint(string subdomain)
        {
            return string.Format(API_ENDPOINT, subdomain, "/users");
        }

        public static string GetAuthStartEndpoint(string subdomain)
        {
            return string.Format(API_ENDPOINT, subdomain, "/auth/start");
        }

        public static string GetConfirmCodeEndpoint(string subdomain)
        {
            return string.Format(API_ENDPOINT, subdomain, "/auth/login");
        }

        public static string GetTokenRefreshEndpoint(string subdomain)
        {
            return string.Format(API_ENDPOINT, subdomain, "/auth/refresh");
        }

        public static string GetAssetEndpoint(string type, int limit, int page, string userId, string appId, string gender)
        {
            if (string.IsNullOrEmpty(type))
            {
                return string.Format(ASSET_ENDPOINT, limit, page, userId, appId, gender);
            }

            return string.Format(ASSET_ENDPOINT, limit, page, userId, appId, gender) + "&type=" + type;
        }

        public static string GetColorEndpoint(string avatarId)
        {
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}/colors?type=skin,beard,hair,eyebrow";
        }

        public static string GetAvatarPublicUrl(string avatarId)
        {
            return $"{MODELS_URL_PREFIX}/{avatarId}.glb";
        }

        public static string GetRenderEndpoint(string avatarId)
        {
            return $"{MODELS_URL_PREFIX}/{avatarId}.png";
        }

        public static string GetUserAvatarsEndpoint(string userId)
        {
            return $"{AVATAR_API_V1_ENDPOINT}?select=id,partner&userId={userId}";
        }

        public static string GetAvatarMetadataEndpoint(string avatarId)
        {
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}.json";
        }

        public static string GetCreateEndpoint()
        {
            return AVATAR_API_V2_ENDPOINT;
        }

        public static string GetAllAvatarTemplatesEndpoint()
        {
            return $"{AVATAR_API_V2_ENDPOINT}/templates";
        }

        public static string GetAvatarTemplatesEndpoint(string templateId)
        {
            return $"{AVATAR_API_V2_ENDPOINT}/templates/{templateId}";
        }

        public static string GetAvatarModelEndpoint(string avatarId, bool isPreview, string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                parameters = parameters.Substring(1);
            }
            
            var previewParamStr = isPreview ? "preview=true&" : "";
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}.glb?{previewParamStr}{parameters}";
        }

        public static string GetUpdateAvatarEndpoint(string avatarId, string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                parameters = parameters.Substring(1);
            }
            
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}?responseType=glb&{parameters}";
        }

        public static string GetSaveAvatarEndpoint(string avatarId)
        {
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}";
        }

        public static string GetDeleteAvatarEndpoint(string avatarId, bool isDraft)
        {
            var draftStr = isDraft ? "draft" : "";
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}/{draftStr}";
        }
    }
}
