using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CreateAssetMenu(fileName = "AvatarCreatorData", menuName = "Scriptable Objects/Ready Player Me/Avatar Creator Data", order = 1)]
    public class AvatarCreatorData : ScriptableObject
    {
        public AvatarProperties AvatarProperties;
        
        public void Awake()
        {
            AvatarProperties = new AvatarProperties();
        }
    }
}
