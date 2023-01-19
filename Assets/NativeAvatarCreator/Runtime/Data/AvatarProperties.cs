using System.Collections.Generic;
using Newtonsoft.Json;

namespace NativeAvatarCreator
{
    public class AvatarProperties
    {
        // Required Fields
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partner;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gender;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BodyType;

        [JsonConverter(typeof(PartnerAssetsDictionaryConverter))]
        public Dictionary<AssetTypeData.PartnerAssetType, object> Assets;
    }
}
