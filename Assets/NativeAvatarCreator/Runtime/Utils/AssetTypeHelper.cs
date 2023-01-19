using System.Collections.Generic;
using System.Linq;

namespace NativeAvatarCreator
{

    public static class AssetTypeHelper
    {
        public static List<PartnerAssetType> GetAssetTypeList()
        {
            return PartnerAssetTypeMap.Select(x => x.Value)
                .Where(x =>
                    x != PartnerAssetType.BeardColor &&
                    x != PartnerAssetType.EyebrowColor &&
                    x != PartnerAssetType.HairColor &&
                    x != PartnerAssetType.FaceStyle)
                .ToList();
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
            { "hairColor", PartnerAssetType.HairColor },
            { "eyebrowColor", PartnerAssetType.EyebrowColor },
            { "beardColor", PartnerAssetType.BeardColor },
            { "faceStyle", PartnerAssetType.FaceStyle },
        };

        public static bool IsFaceAsset(PartnerAssetType assetType)
        {
            switch (assetType)
            {
                case PartnerAssetType.FaceShape:
                case PartnerAssetType.EyeShape:
                case PartnerAssetType.EyeColor:
                case PartnerAssetType.EyebrowStyle:
                case PartnerAssetType.NoseShape:
                case PartnerAssetType.LipShape:
                case PartnerAssetType.BeardStyle:
                    return true;
                default:
                    return false;
            }
        }
    }
}
