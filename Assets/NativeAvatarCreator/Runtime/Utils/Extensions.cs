using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NativeAvatarCreator
{
    public static class Extensions
    {
        public static string ToJson(this Payload payload, bool ignoreEmptyFields = false)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
            };

            if (ignoreEmptyFields)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            return JsonConvert.SerializeObject(payload, settings);
        }
    }
}
