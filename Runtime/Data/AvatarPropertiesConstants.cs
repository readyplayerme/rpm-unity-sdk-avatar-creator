using System.Collections.Generic;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AvatarPropertiesConstants
    {
        public static readonly Dictionary<Category, object> MaleDefaultAssets =
            new Dictionary<Category, object>
            {
                { Category.SkinColor, 5 },
                { Category.EyeColor, "9781796" },
                { Category.HairStyle, "9247476" },
                { Category.EyebrowStyle, "16858292" },
                { Category.Outfit, "109373713" },
                { Category.HairColor, 0 },
                { Category.EyebrowColor, 0 },
                { Category.BeardColor, 0 }
            };
            
        public static readonly Dictionary<Category, object> FemaleDefaultAssets =
            new Dictionary<Category, object>
            {
                { Category.SkinColor, 5 },
                { Category.EyeColor, "9781796" },
                { Category.HairStyle, "9247476" },
                { Category.EyebrowStyle, "16858292" },
                { Category.Outfit, "109376347" },
                { Category.HairColor, 0 },
                { Category.EyebrowColor, 0 },
                { Category.BeardColor, 0 }
            };
    }
}
