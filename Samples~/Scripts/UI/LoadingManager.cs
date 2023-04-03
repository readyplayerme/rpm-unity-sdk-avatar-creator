using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class LoadingManager : MonoBehaviour
    {
        public enum LoadingType
        {
            Fullscreen,
            Popup
        }

        [Serializable]
        private class LoadingData
        {
            public GameObject container;
            public Text tex;
        }

        [SerializeField] private LoadingData fullscreenLoading;
        [SerializeField] private LoadingData popupLoading;

        private LoadingType currentLoadingType;

        public void EnableLoading(string text = "Loading your avatar", LoadingType type = LoadingType.Fullscreen)
        {
            switch (type)
            {
                case LoadingType.Fullscreen:
                    fullscreenLoading.tex.text = text;
                    fullscreenLoading.container.SetActive(true);
                    currentLoadingType = type;
                    break;
                case LoadingType.Popup:
                    popupLoading.tex.text = text;
                    popupLoading.container.SetActive(true);
                    currentLoadingType = type;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void DisableLoading()
        {
            switch (currentLoadingType)
            {
                case LoadingType.Fullscreen:
                    fullscreenLoading.container.SetActive(false);
                    break;
                case LoadingType.Popup:
                    popupLoading.container.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentLoadingType), currentLoadingType, null);
            }
        }
    }
}
