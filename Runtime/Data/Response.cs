using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public struct Response
    {
        public string Text;
        public byte[] Data;
        public Texture Texture;
        public int Size; // In Bytes
        public float Duration;
    }
}
