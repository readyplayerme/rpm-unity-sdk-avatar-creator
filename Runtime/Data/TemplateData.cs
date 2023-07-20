﻿using System;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct TemplateData
    {
        public string ImageUrl;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        public string Id;
    }
}
