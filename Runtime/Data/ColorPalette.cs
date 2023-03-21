using System;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct ColorPalette
    {
        public AssetType assetType;
        public string[] hexColors;

        public ColorPalette(AssetType assetType, string[] hexColors)
        {
            this.assetType = assetType;
            this.hexColors = hexColors;
        }
    }
}