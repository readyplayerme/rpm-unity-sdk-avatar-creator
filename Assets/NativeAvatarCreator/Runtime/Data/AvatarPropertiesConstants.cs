using System.Collections.Generic;

namespace NativeAvatarCreator
{
    public static class AvatarPropertiesConstants
    {
        public const string FULL_BODY = "fullbody";
        public const string HALF_BODY = "halfbody";
        public const string MALE = "male";
        public const string FEMALE = "female";

        public static readonly Dictionary<PartnerAssetType, object> DefaultAssets =
            new Dictionary<PartnerAssetType, object>
            {
                { PartnerAssetType.SkinColor, 5 },
                { PartnerAssetType.EyeColor, "9781796" },
                { PartnerAssetType.HairStyle, "9781796" },
                { PartnerAssetType.EyebrowStyle, "9781796" },
                { PartnerAssetType.Shirt, "9247449" },
                { PartnerAssetType.Outfit, "9781796" },
                { PartnerAssetType.HairColor, 0 },
                { PartnerAssetType.EyebrowColor, 0 },
                { PartnerAssetType.BeardColor, 0 },
            };
    }
}
