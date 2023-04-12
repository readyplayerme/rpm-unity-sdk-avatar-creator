using System;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class ResponseExtension
    {
        public static void ThrowIfError(this Response response)
        {
            if (!response.IsSuccess)
            {
                throw new Exception(response.Text);
            }
        }
    }
}
