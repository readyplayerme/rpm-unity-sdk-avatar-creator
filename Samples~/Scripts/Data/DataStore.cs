using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CreateAssetMenu(fileName = "DataStore", menuName = "Scriptable Objects/Ready Player Me/Data Store", order = 1)]
    public class DataStore : ScriptableObject
    {
        public AvatarProperties AvatarProperties;
        public string AvatarId;
        
        public void Awake()
        {
            AvatarProperties = new AvatarProperties();
        }
    }
}
