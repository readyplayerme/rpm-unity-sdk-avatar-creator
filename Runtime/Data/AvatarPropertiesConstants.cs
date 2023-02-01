using System.Collections.Generic;

namespace NativeAvatarCreator
{
    public static class AvatarPropertiesConstants
    {
        public const string FULL_BODY = "fullbody";
        public const string HALF_BODY = "halfbody";
        public const string MALE = "male";
        public const string FEMALE = "female";

        public static readonly Dictionary<AssetType, object> DefaultAssets =
            new Dictionary<AssetType, object>
            {
                { AssetType.SkinColor, 5 },
                { AssetType.EyeColor, "9781796" },
                { AssetType.HairStyle, "9247476" },
                { AssetType.EyebrowStyle, "16858292" },
                { AssetType.Shirt, "9247449" },
                { AssetType.HairColor, 0 },
                { AssetType.EyebrowColor, 0 },
                { AssetType.BeardColor, 0 },
            };
    }
}
