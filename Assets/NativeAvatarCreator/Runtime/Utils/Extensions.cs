using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NativeAvatarCreator
{
    public static class Extensions
    {
        public static byte[] ToBytes(this Payload payload, bool ignoreEmptyFields = false)
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

            var json = JsonConvert.SerializeObject(payload, settings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
