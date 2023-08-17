using System.Collections.Generic;
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
        public LockedCategories[] LockedCategories;
    }

    public class LockedCategories
    {
        public string Name;
        public KeyValuePair<string, string>[] CustomizationCategories;
    }

    public struct Pagination
    {
        public int TotalDocs;
        public int TotalPages;
        public int Page;
    }
}
