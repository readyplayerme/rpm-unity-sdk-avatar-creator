using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class DataStore : MonoBehaviour
    {
        public UserStore User;
        public AvatarProperties AvatarProperties;

        public void Awake()
        {
            AvatarProperties = new AvatarProperties();
        }
    }
}
