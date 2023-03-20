using System;
using Newtonsoft.Json.Linq;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class ColorResponseHandler
    {
        [Serializable]
        public struct ColorResponse
        {
            public string[] skin;
            public string[] eyebrow;
            public string[] beard;
            public string[] hair;
        }

        private const string SKIN_KEY = "skin";
        private const string EYEBROW_KEY = "eyebrow";
        private const string BEARD_KEY = "beard";
        private const string HAIR_KEY = "hair";
        
        
        public static ColorPalette[] GetColorsFromResponse(string response)
        {
            var responseData = JObject.Parse(response);
            ColorResponse colorResponse = ((JObject) responseData["data"])!.ToObject<ColorResponse>();
            var colorPalettes = new ColorPalette[4];
            colorPalettes[0] = new ColorPalette(SKIN_KEY, colorResponse.skin);
            colorPalettes[1] = new ColorPalette(EYEBROW_KEY, colorResponse.eyebrow);
            colorPalettes[2] = new ColorPalette(BEARD_KEY, colorResponse.beard);
            colorPalettes[3] = new ColorPalette(HAIR_KEY, colorResponse.hair);
            return colorPalettes;
        }
        
        public static AssetType KeyToAssetType(string key)
        {
            return key switch
            {
                EYEBROW_KEY => AssetType.EyebrowColor,
                BEARD_KEY => AssetType.BeardColor,
                HAIR_KEY => AssetType.HairColor,
                _ => AssetType.SkinColor
            };
        }
    }
}
