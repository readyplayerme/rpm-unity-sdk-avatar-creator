using System;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public class TemplateData
    {
        public string ImageUrl;
        [JsonConverter(typeof(GenderConverter))]
        public OutfitGender Gender;
        public string Id;
        public Texture Texture;
    }
}
