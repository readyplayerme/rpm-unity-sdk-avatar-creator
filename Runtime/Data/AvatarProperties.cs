﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct AvatarProperties
    {
        public string Id;
        public string Partner;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        [JsonConverter(typeof(BodyTypeConverter))]
        public BodyType BodyType;
        [JsonConverter(typeof(AssetTypeDictionaryConverter))]
        public Dictionary<AssetType, object> Assets;
        public string Base64Image;
    }
}
