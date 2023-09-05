using Newtonsoft.Json;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public struct AssetData
    {
        public PartnerAsset[] Assets;
        public Pagination Pagination;
    }

    public struct PartnerAsset
    {
        public string Id;
        [JsonProperty("type"), JsonConverter(typeof(CategoryConverter))]
        public Category Category;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        [JsonProperty("iconUrl")]
        public string Icon;
        [JsonProperty("maskUrl")]
        public string Mask;
        [JsonProperty("lockedCategories")]
        public string[] LockedCategories;
    }

    public struct Pagination
    {
        public int TotalPages;
    }
}
