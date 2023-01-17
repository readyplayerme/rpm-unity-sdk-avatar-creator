namespace NativeAvatarCreator
{

    public class Payload
    {
        // Required Fields
        public string Partner;
        public string Gender;
        public string BodyType;

        public PayloadAssets Assets;
    }
    
    public class PayloadAssets
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
