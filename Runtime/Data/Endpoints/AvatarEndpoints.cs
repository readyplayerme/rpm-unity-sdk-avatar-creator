namespace ReadyPlayerMe.AvatarCreator
{
    public abstract class AvatarEndpoints : Endpoints
    {
        private const string AVATAR_API_V2_ENDPOINT = API_V2_ENDPOINT + "avatars";
        private const string AVATAR_API_V1_ENDPOINT = API_V1_ENDPOINT + "avatars";
        private const string MODELS_URL_PREFIX = "https://models.readyplayer.me";

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

            var preview = isPreview ? "preview=true&" : "";
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}.glb?{preview}{parameters}";
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
            var draft = isDraft ? "draft" : "";
            return $"{AVATAR_API_V2_ENDPOINT}/{avatarId}/{draft}";
        }
    }
}
