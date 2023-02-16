using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorSelection : SelectionScreenBase
    {
        [SerializeField] private AssetTypeUICreator assetTypeUICreator;
        [SerializeField] private AssetButtonCreator assetButtonCreator;
        [SerializeField] private Button saveButton;

        public Action Show;
        public Action Hide;

        public Action<string, AssetType> OnClick;
        public Action Save;

        private void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
            saveButton.onClick.AddListener(OnSave);
        }

        private void OnDisable()
        {
            saveButton.gameObject.SetActive(false);
            assetTypeUICreator.ResetUI();
            Hide?.Invoke();
            saveButton.onClick.RemoveListener(OnSave);
        }

        public void CreateUI(BodyType bodyType, Dictionary<string, AssetType> assets)
        {
            assetTypeUICreator.CreateUI(bodyType, AssetTypeHelper.GetAssetTypeList(bodyType));
            assetButtonCreator.CreateUI(assets, OnClick);

            saveButton.gameObject.SetActive(true);
        }

        public void SetAssetIcons(Dictionary<string, Texture> assetIcons)
        {
            assetButtonCreator.SetAssetIcons(assetIcons);
        }

        private void OnSave()
        {
            IsSelected = true;
            Save?.Invoke();
        }
    }
}
