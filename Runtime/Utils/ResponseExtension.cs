using System;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class ResponseExtension
    {
        public static void ThrowIfError(this IResponse response)
        {
            if (!response.IsSuccess)
            {
                throw new Exception(response.Error + "\n" + response.Url);
            }
        }
        
        public static void ThrowIfError(this Response response)
        {
            if (!response.IsSuccess)
            {
                if (!string.IsNullOrEmpty(response.Text))
                {
                    var json = JObject.Parse(response.Text);
                    throw new Exception(json["message"]!.ToString());
                }
                throw new Exception(response.Error);
            }
        }
    }
}
