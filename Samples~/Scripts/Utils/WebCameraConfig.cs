using UnityEngine;

namespace ReadyPlayerMe
{
    [CreateAssetMenu(fileName = "WebCameraConfig", menuName = "Scriptable Objects/Ready Player Me/Web Camera Config", order = 3)]
    public class WebCameraConfig : ScriptableObject
    {
        [SerializeField, WebCameraDevice] public string device;
    }
}
