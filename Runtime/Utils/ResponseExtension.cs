using System;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class ResponseExtension
    {
        public static void ThrowIfError(this IResponse response)
        {
            if (!response.IsSuccess)
            {
                throw new Exception(response.Error);
            }
        }
    }
}
