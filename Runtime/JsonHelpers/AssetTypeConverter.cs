using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NativeAvatarCreator
{
    public class AssetTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AssetType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return AssetTypeHelper.PartnerAssetTypeMap[token.ToString()];
            }
            throw new JsonSerializationException("Expected string value");
        }
    }
}
