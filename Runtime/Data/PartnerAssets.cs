using System.Collections.Generic;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.AvatarCreator
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
        public LockedCategories[] LockedCategories;
    }

    public struct LockedCategories
    {
        public string Name;
        public KeyValuePair<string, string>[] CustomizationCategories;
    }
}
