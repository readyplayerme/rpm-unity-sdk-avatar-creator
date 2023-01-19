using System.Collections.Generic;

namespace NativeAvatarCreator
{
    public static class AvatarPropertiesConstants
    {
        public const string FULL_BODY = "fullbody";
        public const string HALF_BODY = "halfbody";
        public const string MALE = "male";
        public const string FEMALE = "female";
        
        public static readonly Dictionary<AssetTypeData.PartnerAssetType, object> DefaultAssets =
            new Dictionary<AssetTypeData.PartnerAssetType, object>
            {
                { AssetTypeData.PartnerAssetType.SkinColor, 5 },
                { AssetTypeData.PartnerAssetType.EyeColor, "9781796" },
                { AssetTypeData.PartnerAssetType.HairStyle, "9781796" },
                { AssetTypeData.PartnerAssetType.EyebrowStyle, "9781796" },
                { AssetTypeData.PartnerAssetType.Shirt, "9247449" },
                { AssetTypeData.PartnerAssetType.Outfit, "9781796" },
                { AssetTypeData.PartnerAssetType.HairColor, 0 },
                { AssetTypeData.PartnerAssetType.EyebrowColor, 0 },
                { AssetTypeData.PartnerAssetType.BeardColor, 0 },
            };
    }
}
