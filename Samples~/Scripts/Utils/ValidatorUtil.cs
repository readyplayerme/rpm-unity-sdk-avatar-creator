using System.Text.RegularExpressions;

namespace ReadyPlayerMe
{
    public static class ValidatorUtil
    {
        private const string EMAIL_REGEX = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                           + "@"
                                           + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        public static bool IsValidEmail(string email)
        {
            var regex = new Regex(EMAIL_REGEX);
            var match = regex.Match(email);
            return match.Success;
        }
    }
}
