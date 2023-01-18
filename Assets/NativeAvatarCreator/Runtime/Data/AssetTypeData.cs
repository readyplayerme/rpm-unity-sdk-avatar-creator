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

        public static readonly Dictionary<string, PartnerAssetType> PartnerAssetTypeEnumDictionary = new Dictionary<string, PartnerAssetType>
        {
            { "faceshape", PartnerAssetType.FaceShape },
            { "eyeshape", PartnerAssetType.EyeShape },
            { "eye", PartnerAssetType.EyeColor },
            { "eyebrows", PartnerAssetType.EyebrowStyle },
            { "noseshape", PartnerAssetType.NoseShape },
            { "lipshape", PartnerAssetType.LipShape },
            { "beard", PartnerAssetType.BeardStyle },
            { "facemask", PartnerAssetType.FaceMask },
            { "glasses", PartnerAssetType.Glasses },
            { "hair", PartnerAssetType.HairStyle },
            { "headwear", PartnerAssetType.Headwear },
            { "facewear", PartnerAssetType.Facewear },
            { "outfit", PartnerAssetType.Outfit },
            { "shirt", PartnerAssetType.Shirt },
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
