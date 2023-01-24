using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AvatarCreatorSelection : SelectionScreen
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private AssetTypeUICreator assetTypeUICreator;
        [SerializeField] private Button saveButton;

        public Action Show;
        public Action Hide;

        private Dictionary<AssetType, AssetButton> selectedAssetByTypeMap;

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

        public void AddAllAssetButtons(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, AssetType> onClick, Action onSave)
        {
            selectedAssetByTypeMap = new Dictionary<AssetType, AssetButton>();
            var distinctAssetType = assets.Keys.Select(x => x.AssetType).Distinct();
            assetTypeUICreator.CreateUI(AssetTypeHelper.GetAssetTypeList().Where(x => distinctAssetType.Contains(x)));

            var orderedAssets = assets.OrderByDescending(x => x.Key.AssetType == AssetType.FaceShape);
            foreach (var asset in orderedAssets)
            {
                var parent = PanelSwitcher.AssetTypePanelMap[asset.Key.AssetType];
                AddAssetButton(asset.Key.Id, parent.transform, asset.Key.AssetType, onClick, asset.Value);
            }

            foreach (var assetTypePanelMap in PanelSwitcher.AssetTypePanelMap)
            {
                var assetType = assetTypePanelMap.Key;
                var assetTypePanel = assetTypePanelMap.Value;
                if (assetType != AssetType.Outfit && assetType != AssetType.Shirt)
                {
                    AddAssetSelectionClearButton(assetTypePanel.transform, assetType, onClick);
                }
            }

            saveButton.gameObject.SetActive(true);
            saveButton.onClick.AddListener(() =>
            {
                IsSelected = true;
                onSave?.Invoke();
            });
            Loading.SetActive(false);
        }

        private async void AddAssetButton(string assetId, Transform parent, AssetType assetType, Action<string, AssetType> onClick,
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

        private void AddAssetSelectionClearButton(Transform parent, AssetType assetType, Action<string, AssetType> onClick)
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
