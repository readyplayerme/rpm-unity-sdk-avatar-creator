using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AssetTypeDictionaryConverter : JsonConverter<Dictionary<AssetType, object>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<AssetType, object> value, JsonSerializer serializer)
        {
            var newValue = new Dictionary<string, object>();
            foreach (var element in value)
            {
                var key = element.Key.ToString();
                var camelCaseKey = char.ToLowerInvariant(key[0]) + key.Substring(1);
                newValue.Add(camelCaseKey, element.Value);
            }

            serializer.Serialize(writer, newValue);
        }

        public override Dictionary<AssetType, object> ReadJson(JsonReader reader, Type objectType,
            Dictionary<AssetType, object> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var assets = new Dictionary<AssetType, object>();
            if (token.Type == JTokenType.Object)
            {
                foreach (var element in token.ToObject<Dictionary<string, object>>())
                {
                    if (element.Key == "createdAt" || element.Key == "updatedAt" || element.Key == "skinColorHex")
                    {
                        continue;
                    }

                    var pascalCaseKey = char.ToUpperInvariant(element.Key[0]) + element.Key.Substring(1);
                    var assetType = (AssetType) Enum.Parse(typeof(AssetType), pascalCaseKey);
                    assets.Add(assetType, element.Value);
                }
            }

            return assets;
        }
    }
}
