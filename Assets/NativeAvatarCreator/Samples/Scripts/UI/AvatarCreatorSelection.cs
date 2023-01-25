using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AvatarCreatorSelection : SelectionScreen
    {
        [SerializeField] private AssetTypeUICreator assetTypeUICreator;
        [SerializeField] private AssetButtonCreator assetButtonCreator;
        [SerializeField] private Button saveButton;
        
        public Action Show;
        public Action Hide;

        private void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        private void OnDisable()
        {
            saveButton.gameObject.SetActive(false);
            assetTypeUICreator.ResetUI();
            Hide?.Invoke();
        }

        public void AddAllAssetButtons(BodyType bodyType, Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, AssetType> onClick,
            Action onSave)
        {
            var distinctAssetType = assets.Keys.Select(x => x.AssetType).Distinct();
            assetTypeUICreator.CreateUI(bodyType, AssetTypeHelper.GetAssetTypeList().Where(x => distinctAssetType.Contains(x)));

            var orderedAssets = assets.OrderByDescending(x => x.Key.AssetType == AssetType.FaceShape);
            assetButtonCreator.CreateUI(orderedAssets, onClick);

            saveButton.gameObject.SetActive(true);
            saveButton.onClick.AddListener(() =>
            {
                IsSelected = true;
                onSave?.Invoke();
            });
        }
    }
}
