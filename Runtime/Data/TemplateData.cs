﻿using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.AvatarCreator
{
    public struct TemplateData
    {
        public string ImageUrl;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        public string Id;
    }
}
