using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AssetButtonCreator : MonoBehaviour
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;

        private Dictionary<AssetType, AssetButton> selectedAssetByTypeMap;
        private Dictionary<string, AssetButton> assetMap;

        public void CreateUI(IEnumerable<KeyValuePair<string, AssetType>> assets, Action<string, AssetType> onClick)
        {
            selectedAssetByTypeMap = new Dictionary<AssetType, AssetButton>();
            assetMap = new Dictionary<string, AssetButton>();
            
            foreach (var asset in assets)
            {
                var parent = PanelSwitcher.AssetTypePanelMap[asset.Value];
                AddAssetButton(asset.Key, parent.transform, asset.Value, onClick);
            }

            foreach (var assetTypePanelMap in PanelSwitcher.AssetTypePanelMap)
            {
                var assetType = assetTypePanelMap.Key;
                var assetTypePanel = assetTypePanelMap.Value;
                if (assetType != AssetType.Outfit && assetType != AssetType.Shirt && assetType != AssetType.EyeColor)
                {
                    AddAssetSelectionClearButton(assetTypePanel.transform, assetType, onClick);
                }
            }
        }

        public void SetAssetIcons(Dictionary<string, Texture> assetIcons)
        {
            foreach (var asset in assetIcons)
            {
                assetMap[asset.Key].SetIcon(asset.Value);
            }
        }

        private async void AddAssetButton(string assetId, Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.name = "Asset-" + assetId;
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
            if (assetType == AssetType.EyeColor)
            {
                assetButton.SetEyeColorConfig(assetType);
            }
            
            assetMap.Add(assetId, assetButton);
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
