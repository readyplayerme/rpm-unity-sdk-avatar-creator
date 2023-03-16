using System;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct ColorPalette
    {
        public AssetType assetType;
        public string[] hexColors;

        public ColorPalette(string name, string[] colorHex)
        {
            hexColors = new string[colorHex.Length];
            for (var i = 0; i < colorHex.Length; i++)
            {
                hexColors[i] = colorHex[i];
            }
            assetType = AssetType.SkinColor;
            switch (name)
            {
                case "skin":
                    assetType = AssetType.SkinColor;
                    break;
                case "eyebrow":
                    assetType = AssetType.EyebrowColor;
                    break;
                case "beard":
                    assetType = AssetType.BeardColor;
                    break;
                case "hair":
                    assetType = AssetType.HairColor;
                    break;
            }
        }
    }
}