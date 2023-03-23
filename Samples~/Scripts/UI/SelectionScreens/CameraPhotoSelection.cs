using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class CameraPhotoSelection : State
    {
        private const string IGNORE_CAMERA_SUBSTRING = "OBS";
        
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Button cameraButton;
        [SerializeField] private Button fileButton;
        
        // TODO use config to determine correct camera device
        [SerializeField] private WebCameraConfig webCameraConfig;
        public override StateType StateType => StateType.CameraPhoto;
        private WebCamTexture camTexture;
        
        private void OnEnable()
        {
            cameraButton.onClick.AddListener(OnCameraButton);
            fileButton.onClick.AddListener(OnFileButton);
            OpenCamera();
        }

        private void OnDisable()
        {
            cameraButton.onClick.RemoveListener(OnCameraButton);
            fileButton.onClick.RemoveListener(OnFileButton);
            CloseCamera();
        }

        private void OpenCamera()
        {
            var devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                return;
            }

            rawImage.color = Color.white;
            foreach (var device in devices)
            {
                // TODO Find a better approach to select correct camera device
                if (device.name.Contains(IGNORE_CAMERA_SUBSTRING))
                {
                    if (device.name != webCameraConfig.device)
                        continue;
                }

                var size = rawImage.rectTransform.sizeDelta;
                camTexture = new WebCamTexture(device.name, (int) size.x, (int) size.y);
                camTexture.Play();
                rawImage.texture = camTexture;
                rawImage.SetNativeSize();
                return;
            }
        }

        private void CloseCamera()
        {
            if (camTexture != null && camTexture.isPlaying)
            {
                camTexture.Stop();
            }
        }

        private void OnFileButton()
        {
            // Will not be implemented
        }

        private void OnCameraButton()
        {
            var texture = new Texture2D(rawImage.texture.width, rawImage.texture.height, TextureFormat.ARGB32, false);
            texture.SetPixels(camTexture.GetPixels());
            texture.Apply();

            var bytes = texture.EncodeToPNG();
            DataStore.AvatarProperties.Base64Image = Convert.ToBase64String(bytes);

            StateMachine.SetState(StateType.Editor);
        }
    }
}
