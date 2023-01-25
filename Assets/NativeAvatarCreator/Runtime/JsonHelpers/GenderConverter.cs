using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.AvatarLoader;

namespace NativeAvatarCreator
{
    public class GenderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var newValue = value switch
            {
                OutfitGender.Masculine => "male",
                OutfitGender.Feminine => "female",
                OutfitGender.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            serializer.Serialize(writer, newValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                switch (token.ToString())
                {
                    case "male":
                        return OutfitGender.Masculine;
                    case "female":
                        return OutfitGender.Feminine;
                    default:
                        return OutfitGender.None;
                }
            }

            throw new JsonSerializationException("Expected string value: " + token.Type + " " + token.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OutfitGender);
        }
    }
}
