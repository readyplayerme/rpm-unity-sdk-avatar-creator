using Newtonsoft.Json;

namespace NativeAvatarCreator
{
    public struct PartnerAsset
    {
        public string Id;
        [JsonConverter(typeof(AssetTypeConverter))]
        public AssetType AssetType;
        public string Gender;
        public string Icon;
    }
}
