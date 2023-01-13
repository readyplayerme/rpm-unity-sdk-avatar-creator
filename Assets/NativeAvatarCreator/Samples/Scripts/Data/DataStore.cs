using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class DataStore : MonoBehaviour
    {
        public UserStore User;
        public Payload Payload;

        public void Awake()
        {
            Payload = new Payload();
        }
    }
}
