using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NativeAvatarCreator
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
            throw new NotImplementedException();
        }
    }
}
