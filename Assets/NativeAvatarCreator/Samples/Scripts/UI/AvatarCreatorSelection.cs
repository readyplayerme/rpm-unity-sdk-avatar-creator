using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AvatarCreatorSelection : SelectionPanel
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private AssetTypeUICreator assetTypeUICreator;

        public Action Show;
        public Action Hide;

        private Dictionary<PartnerAssetType, AssetButton> selectedAssetByTypeMap;

        private void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        private void OnDisable()
        {
            Hide?.Invoke();
        }

        public void AddAllAssetButtons(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, PartnerAssetType> onClick)
        {
            selectedAssetByTypeMap = new Dictionary<PartnerAssetType, AssetButton>();
            foreach (var asset in assets)
            {
                var parent = assetTypeUICreator.AssetTypePanelsMap[asset.Key.AssetType];
                AddAssetButton(asset.Key.Id, parent, asset.Key.AssetType, onClick, asset.Value);
            }

            foreach (var assetTypePanelMap in assetTypeUICreator.AssetTypePanelsMap)
            {
                var assetType = assetTypePanelMap.Key;
                var assetTypePanel = assetTypePanelMap.Value;
                if (assetType != PartnerAssetType.Outfit && assetType != PartnerAssetType.Shirt)
                {
                    AddAssetSelectionClearButton(assetTypePanel, assetType, onClick);
                }

                // Disable the asset type which doesn't have any assets.
                var scrollRect = assetTypePanel.GetComponent<ScrollRect>();
                assetTypeUICreator.AssetTypeButtonsMap[assetType].gameObject.SetActive(scrollRect.content.childCount != 0);
            }

            Loading.SetActive(false);
        }

        private async void AddAssetButton(string assetId, Transform parent, PartnerAssetType assetType, Action<string, PartnerAssetType> onClick,
            Task<Texture> iconDownloadTask)
        {
            var assetButtonGameObject = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                if (selectedAssetByTypeMap.ContainsKey(assetType))
                {
                    selectedAssetByTypeMap[assetType].SetSelect(false);
                    selectedAssetByTypeMap[assetType] = assetButton;
                }
                else
                {
                    selectedAssetByTypeMap.Add(assetType, assetButton);
                }

                assetButton.SetSelect(true);
                onClick?.Invoke(assetId, assetType);
            });
            assetButton.SetIcon(await iconDownloadTask);
        }

        private void AddAssetSelectionClearButton(Transform parent, PartnerAssetType assetType, Action<string, PartnerAssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetAsFirstSibling();
            assetButtonGameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedAssetByTypeMap.ContainsKey(assetType))
                {
                    selectedAssetByTypeMap[assetType].SetSelect(false);
                }

                onClick?.Invoke(string.Empty, assetType);
            });
        }
    }
}
