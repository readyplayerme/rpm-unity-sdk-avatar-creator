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
            assetType = ColorResponseHandler.KeyToAssetType(name);
        }
    }
}