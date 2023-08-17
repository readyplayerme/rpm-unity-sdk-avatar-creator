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

        public static ColorPalette[] GetColorsFromResponse(string response)
        {
            var responseData = JObject.Parse(response);
            ColorResponse colorResponse = ((JObject) responseData["data"])!.ToObject<ColorResponse>();
            return ResponseToColorPalettes(colorResponse);
        }

        private static ColorPalette[] ResponseToColorPalettes(ColorResponse colorResponse)
        {
            var colorPalettes = new ColorPalette[4];
            colorPalettes[0] = new ColorPalette(Category.SkinColor, colorResponse.skin);
            colorPalettes[1] = new ColorPalette(Category.EyebrowColor, colorResponse.eyebrow);
            colorPalettes[2] = new ColorPalette(Category.BeardColor, colorResponse.beard);
            colorPalettes[3] = new ColorPalette(Category.HairColor, colorResponse.hair);
            return colorPalettes;
        }
    }
}
