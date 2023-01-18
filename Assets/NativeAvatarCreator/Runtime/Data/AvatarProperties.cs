using System.Collections.Generic;
using Newtonsoft.Json;

namespace NativeAvatarCreator
{
    public class AvatarProperties
    {
        public const string FULL_BODY = "fullbody";
        public const string HALF_BODY = "halfbody";
        public const string MALE = "male";
        public const string FEMALE = "female";
        
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
