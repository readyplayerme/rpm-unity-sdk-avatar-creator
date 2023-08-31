using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public class CategoryDictionaryConverter : JsonConverter<Dictionary<Category, object>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<Category, object> value, JsonSerializer serializer)
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

        public override Dictionary<Category, object> ReadJson(JsonReader reader, Type objectType,
            Dictionary<Category, object> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var assets = new Dictionary<Category, object>();
            if (token.Type == JTokenType.Object)
            {
                foreach (var element in token.ToObject<Dictionary<string, object>>())
                {
                    if (CanSkipProperty(element.Key))
                    {
                        continue;
                    }

                    var pascalCaseKey = char.ToUpperInvariant(element.Key[0]) + element.Key.Substring(1);
                    if (!Enum.IsDefined(typeof(Category), pascalCaseKey))
                    {
                        continue;
                    }
                    var category = (Category) Enum.Parse(typeof(Category), pascalCaseKey);
                    assets.Add(category, element.Value);
                }
            }

            return assets;
        }
        
        private bool CanSkipProperty(string propertyName)
        {
            return propertyName == "createdAt" || propertyName == "updatedAt" || propertyName == "skinColorHex";
        }
    }
}
