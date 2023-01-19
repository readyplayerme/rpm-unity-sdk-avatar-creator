using System.Collections.Generic;

namespace NativeAvatarCreator
{
    public static class AssetTypeData
    {
        public enum PartnerAssetType
        {
            None,
            SkinColor,
            BeardStyle,
            EyeColor,
            EyeShape,
            EyebrowStyle,
            FaceMask,
            FaceShape,
            Glasses,
            HairStyle,
            Headwear,
            Facewear,
            LipShape,
            NoseShape,
            Outfit,
            Shirt,
            HairColor,
            EyebrowColor,
            BeardColor,
            FaceStyle,
        }

        public static readonly Dictionary<string, PartnerAssetType> PartnerAssetTypeMap = new Dictionary<string, PartnerAssetType>
        {
            { "faceshape", PartnerAssetType.FaceShape },
            { "eyeshape", PartnerAssetType.EyeShape },
            { "eye", PartnerAssetType.EyeColor },
            { "eyebrows", PartnerAssetType.EyebrowStyle },
            { "noseshape", PartnerAssetType.NoseShape },
            { "lipshape", PartnerAssetType.LipShape },
            { "beard", PartnerAssetType.BeardStyle },
            { "hair", PartnerAssetType.HairStyle },
            { "outfit", PartnerAssetType.Outfit },
            { "shirt", PartnerAssetType.Shirt },
            { "glasses", PartnerAssetType.Glasses },
            { "facemask", PartnerAssetType.FaceMask },
            { "facewear", PartnerAssetType.Facewear },
            { "headwear", PartnerAssetType.Headwear },
        };

        public static bool IsFaceAsset(string assetType)
        {
            switch (assetType)
            {
                case "faceshape":
                case "eyeshape":
                case "eye":
                case "eyebrows":
                case "noseshape":
                case "lipshape":
                case "beard":
                    return true;
                default:
                    return false;
            }
        }
    }
}
