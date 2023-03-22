using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CustomPropertyDrawer(typeof(WebCameraDeviceAttribute))]
    public class CameraDevicesDrawer : PropertyDrawer
    {
        private readonly string[] devices;
        private int choiceIndex;

        public CameraDevicesDrawer()
        {
            var webCamDevices = WebCamTexture.devices;
            devices = new string[webCamDevices.Length];
            for (var i = 0; i < webCamDevices.Length; i++)
            {
                devices[i] = webCamDevices[i].name;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var labelPosition = EditorGUI.PrefixLabel(position, new GUIContent("Devices"));
            choiceIndex = EditorGUI.Popup(labelPosition, choiceIndex, devices);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = devices[choiceIndex];
            }
        }

    }
}
