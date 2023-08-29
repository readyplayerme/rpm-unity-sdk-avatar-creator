using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Category);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type != JTokenType.String)
            {
                throw new JsonSerializationException("Expected string value");
            }
            
            if (!CategoryHelper.PartnerCategoryMap.ContainsKey(token.ToString()))
            {
                return Category.None;
            }
                
            return CategoryHelper.PartnerCategoryMap[token.ToString()];
        }
    }
}
