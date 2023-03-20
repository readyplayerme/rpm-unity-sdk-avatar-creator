using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class CameraPhotoSelection : State
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Button cameraButton;
        [SerializeField] private Button fileButton;
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

            foreach (var device in devices)
            {
                if (device.name.Contains("OBS"))
                {
                    continue;
                }

                var size = rawImage.rectTransform.sizeDelta;
                camTexture = new WebCamTexture(device.name, (int) size.x, (int) size.y);
                camTexture.Play();
                rawImage.texture = camTexture;
            }
        }

        private void CloseCamera()
        {
            if (camTexture.isPlaying)
            {
                camTexture.Stop();
            }
        }

        private void OnFileButton()
        {
            throw new NotImplementedException();
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
