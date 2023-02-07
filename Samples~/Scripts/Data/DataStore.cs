using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class DataStore : MonoBehaviour
    {
        public UserSession User;
        public AvatarProperties AvatarProperties;

        public void Awake()
        {
            AvatarProperties = new AvatarProperties();
        }
    }
}
