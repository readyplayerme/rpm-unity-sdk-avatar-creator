﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct AvatarProperties
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Partner;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;

        [JsonConverter(typeof(BodyTypeConverter))]
        public BodyType BodyType;

        [JsonConverter(typeof(AssetTypeDictionaryConverter))]
        public Dictionary<AssetType, object> Assets;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Base64Image;
    }
}
