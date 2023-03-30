using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class LoadingManager : MonoBehaviour
    {
        public enum LoadingType
        {
            A,
            B
        }

        [Serializable]
        private class LoadingData
        {
            public GameObject container;
            public Text tex;
        }

        [SerializeField] private LoadingData loadingA;
        [SerializeField] private LoadingData loadingB;

        private LoadingType currentLoadingType;

        public void EnableLoading(string text = "Loading your avatar", LoadingType type = LoadingType.A)
        {
            switch (type)
            {
                case LoadingType.A:
                    loadingA.tex.text = text;
                    loadingA.container.SetActive(true);
                    currentLoadingType = type;
                    break;
                case LoadingType.B:
                    loadingB.tex.text = text;
                    loadingB.container.SetActive(true);
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
                case LoadingType.A:
                    loadingA.container.SetActive(false);
                    break;
                case LoadingType.B:
                    loadingB.container.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentLoadingType), currentLoadingType, null);
            }
        }
    }
}
