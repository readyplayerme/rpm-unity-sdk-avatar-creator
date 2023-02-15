using System.Collections.Generic;
using System.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AssetTypeHelper
    {
        public static IEnumerable<AssetType> GetAssetTypeList()
        {
            return PartnerAssetTypeMap.Select(x => x.Value)
                .Where(x =>
                    x != AssetType.BeardColor &&
                    x != AssetType.EyebrowColor &&
                    x != AssetType.HairColor &&
                    x != AssetType.FaceStyle)
                .ToList();
        }

        public static readonly Dictionary<string, AssetType> PartnerAssetTypeMap = new Dictionary<string, AssetType>
        {
            { "faceshape", AssetType.FaceShape },
            { "eyeshape", AssetType.EyeShape },
            { "eye", AssetType.EyeColor },
            { "eyebrows", AssetType.EyebrowStyle },
            { "noseshape", AssetType.NoseShape },
            { "lipshape", AssetType.LipShape },
            { "beard", AssetType.BeardStyle },
            { "hair", AssetType.HairStyle },
            { "outfit", AssetType.Outfit },
            { "shirt", AssetType.Shirt },
            { "glasses", AssetType.Glasses },
            { "facemask", AssetType.FaceMask },
            { "facewear", AssetType.Facewear },
            { "headwear", AssetType.Headwear },
            { "hairColor", AssetType.HairColor },
            { "eyebrowColor", AssetType.EyebrowColor },
            { "beardColor", AssetType.BeardColor },
            { "faceStyle", AssetType.FaceStyle },
        };

        public static bool IsFaceAsset(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.FaceShape:
                case AssetType.EyeShape:
                case AssetType.EyeColor:
                case AssetType.EyebrowStyle:
                case AssetType.NoseShape:
                case AssetType.LipShape:
                case AssetType.BeardStyle:
                    return true;
                default:
                    return false;
            }
        }
    }
}
