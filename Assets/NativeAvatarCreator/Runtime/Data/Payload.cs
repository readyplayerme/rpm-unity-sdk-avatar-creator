namespace NativeAvatarCreator
{
    public struct Payload
    {
        public Data Data;
    }

    public struct Data
    {
        // Required Fields
        public string Partner;
        public string Gender;
        public string BodyType;

        public Assets Assets;
    }

    public struct Assets
    {
        public int SkinColor;
        public string EyeColor;
        public string HairStyle;
        public int HairColor;
        public string BeardStyle;
        public int BeardColor;
        public string EyebrowStyle;
        public int EyebrowColor;
        public string Shirt;
        public string Outfit;
        public string Glasses;
        public string FaceMask;
        public string Headwear;
        public string Facewear;
        public string LipShape;
        public string EyeShape;
        public string NoseShape;
        public string FaceShape;
        public string FaceStyle;
    }
}


