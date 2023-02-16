using System;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;

namespace ReadyPlayerMe.AvatarCreator
{
    public class BodyTypeConverter : JsonConverter<BodyType>
    {
        public override void WriteJson(JsonWriter writer, BodyType value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString().ToLower());
        }

        public override BodyType ReadJson(JsonReader reader, Type objectType, BodyType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
