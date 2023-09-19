using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class StringExtension
    {
        public static string PascalToCamelCase(this string pascalCaseString)
        {
            if (string.IsNullOrEmpty(pascalCaseString))
            {
                return pascalCaseString;
            }

            return char.ToLowerInvariant(pascalCaseString[0]) + pascalCaseString.Substring(1);
        }
    }
}
