namespace ReadyPlayerMe.AvatarCreator
{
    public abstract class AssetEndpoints : Endpoints
    {
        private const string ASSET_ENDPOINT = API_V1_ENDPOINT + "assets?limit={0}&page={1}&filter=viewable-by-user-and-app&filterUserId={2}&filterApplicationId={3}&gender=neutral&gender={4}";

        public static string GetAssetEndpoint(string type, int limit, int page, string userId, string appId, string gender)
        {
            if (string.IsNullOrEmpty(type))
            {
                return string.Format(ASSET_ENDPOINT, limit, page, userId, appId, gender);
            }

            return string.Format(ASSET_ENDPOINT, limit, page, userId, appId, gender) + "&type=" + type;
        }
    }
}
