using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public class BodyTypeConverter : JsonConverter<BodyType>
    {
        private const string FULL_BODY = "fullbody";
        private const string HALF_BODY = "halfbody";

        public override void WriteJson(JsonWriter writer, BodyType value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString().ToLower());
        }

        public override BodyType ReadJson(JsonReader reader, Type objectType, BodyType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return token.ToString() switch
                {
                    FULL_BODY => BodyType.FullBody,
                    HALF_BODY => BodyType.HalfBody,
                    _ => BodyType.None
                };
            }

            throw new JsonSerializationException("Expected string value, instead found: " + token.Type);
        }
    }
}
