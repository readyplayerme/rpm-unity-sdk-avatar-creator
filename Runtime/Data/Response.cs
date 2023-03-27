using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public struct Response
    {
        public bool IsSuccess;
        public long ResponseCode;
        public string Text;
        public byte[] Data;
        public Texture Texture;
        public int Size; // In Bytes
        public float Duration;
    }
}
