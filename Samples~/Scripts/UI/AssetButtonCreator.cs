using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;
using ColorPalette = ReadyPlayerMe.AvatarCreator.ColorPalette;

namespace ReadyPlayerMe
{
    public class AssetButtonCreator : MonoBehaviour
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private GameObject colorAssetButtonPrefab;

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
                if (assetType != AssetType.Outfit && assetType != AssetType.Shirt && !assetType.IsColorAsset())
                {
                    AddAssetSelectionClearButton(assetTypePanel.transform, assetType, onClick);
                }
            }
        }
        
        public void CreateColorUI(ColorPalette[] colorPalettes, Action<AssetType, int> onClick)
        {
            foreach (var colorPalette in colorPalettes)
            {
                var parent = PanelSwitcher.AssetTypePanelMap[colorPalette.assetType];
                var assetIndex = 0;
                foreach (var color in colorPalette.hexColors)
                {
                    var button = AddColorButton(assetIndex, parent.transform, colorPalette.assetType, onClick);
                    button.SetColor(color);
                    // By default first color is applied on initial draft
                    if (assetIndex == 0)
                    {
                        button.SetSelect(true);
                    }
                    assetIndex++;
                }
            }
        }

        public void SetAssetIcons(Dictionary<string, Texture> assetIcons)
        {
            foreach (var asset in assetIcons)
            {
                if (assetMap.ContainsKey(asset.Key))
                {
                    assetMap[asset.Key].SetIcon(asset.Value);
                }
            }
        }
        
        private AssetButton AddColorButton(int index, Transform parent, AssetType assetType, Action<AssetType, int> onClick)
        {
            var assetButtonGameObject = Instantiate(colorAssetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.name = $"{assetType}_{index}";
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
                onClick?.Invoke(assetType, index);
            });
            assetMap.Add($"{assetType}_{index}", assetButton);
            return assetButton;
        }

        private void AddAssetButton(string assetId, Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            if (assetMap.ContainsKey(assetId)) return;
            
            var assetButtonGameObject = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.name = "Asset-" + assetId;
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(assetId, assetType, onClick, assetButton);
            });
            if (assetType == AssetType.EyeColor)
            {
                assetButton.SetEyeColorConfig();
            }
            
            assetMap.Add(assetId, assetButton);
        }

        private void SelectButton(string assetId, AssetType assetType, Action<string, AssetType> onClick, AssetButton assetButton)
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
        }

        private void AddAssetSelectionClearButton(Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetAsFirstSibling();
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(string.Empty, assetType, onClick, assetButton);
            });
            // Clear is selected initially by default
            assetButton.SetSelect(true);
        }

    }
}
