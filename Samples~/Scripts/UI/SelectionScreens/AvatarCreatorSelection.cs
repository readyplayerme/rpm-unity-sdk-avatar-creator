using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void AddAllAssetButtons(BodyType bodyType, Dictionary<PartnerAsset, Task<Texture>> assets)
        {
            var distinctAssetType = assets.Keys.Select(x => x.AssetType).Distinct();
            assetTypeUICreator.CreateUI(bodyType, AssetTypeHelper.GetAssetTypeList().Where(x => distinctAssetType.Contains(x)));

            var orderedAssets = assets.OrderByDescending(x => x.Key.AssetType == AssetType.FaceShape);
            assetButtonCreator.CreateUI(orderedAssets, OnClick);

            saveButton.gameObject.SetActive(true);
        }

        private void OnSave()
        {
            IsSelected = true;
            Save?.Invoke();
        }
    }
}
