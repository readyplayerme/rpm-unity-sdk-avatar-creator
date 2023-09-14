using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarPropertiesExtensions
    {
        public static string ToJson(this AvatarProperties avatarProperties, bool ignoreEmptyFields = false)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };

            var data = new Dictionary<string, AvatarProperties>
            {
                { "data", avatarProperties }
            };

            if (ignoreEmptyFields)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            return JsonConvert.SerializeObject(data, settings);
        }

        public static string GetPrecompilePayload(this AvatarProperties avatarProperties)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };

            var dataContainer = new Dictionary<string, string[]>();
            foreach (var keyValuePair in avatarProperties.Assets)
            {
                if (string.IsNullOrEmpty(keyValuePair.Value.ToString())) continue;
                var key = keyValuePair.Key.ToString();
                var camelCaseKey = $"{char.ToLowerInvariant(key[0])}{key.Substring(1)}";
                dataContainer.Add(camelCaseKey, new[] { keyValuePair.Value.ToString() });
            }

            var data = new Dictionary<string, Dictionary<string, string[]>>
            {
                { "data", dataContainer }
            };

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;

            return JsonConvert.SerializeObject(data, settings);
        }
    }
}
