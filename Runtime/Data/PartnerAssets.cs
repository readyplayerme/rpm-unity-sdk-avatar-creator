using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace AvatarCreator
{
    public struct PartnerAsset
    {
        public string Id;
        [JsonConverter(typeof(AssetTypeConverter))]
        public AssetType AssetType;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        public string Icon;
        public string Mask;
    }
}
