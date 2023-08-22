using System;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct ColorPalette
    {
        public Category category;
        public string[] hexColors;

        public ColorPalette(Category category, string[] hexColors)
        {
            this.category = category;
            this.hexColors = hexColors;
        }
    }
}