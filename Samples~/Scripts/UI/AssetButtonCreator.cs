using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AssetButtonCreator : MonoBehaviour
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private GameObject colorAssetButtonPrefab;

        private Dictionary<AssetType, AssetButton> selectedAssetByTypeMap;
        private Dictionary<object, AssetButton> assetMap;

        public void CreateAssetButtons(IEnumerable<KeyValuePair<string, AssetType>> assets, Action<string, AssetType> onClick)
        {
            selectedAssetByTypeMap = new Dictionary<AssetType, AssetButton>();
            assetMap = new Dictionary<object, AssetButton>();
            
            foreach (var asset in assets)
            {
                var parent = PanelSwitcher.AssetTypePanelMap[asset.Value];
                AddAssetButton(asset.Key, parent.transform, asset.Value, onClick);
            }
        }

        public void CreateClearButton(Action<string, AssetType> onClick)
        {
            foreach (var assetTypePanelMap in PanelSwitcher.AssetTypePanelMap)
            {
                var assetType = assetTypePanelMap.Key;
                if (assetType.IsOptionalAsset())
                {
                    var assetTypePanel = assetTypePanelMap.Value;
                    AddAssetSelectionClearButton(assetTypePanel.transform, assetType, onClick);
                }
            }
        }

        public void SetSelectedAssets(Dictionary<AssetType, object> selected)
        {
            foreach (var asset in selected)
            {
                var assetType = asset.Key;
                var assetId = asset.Value;
                if (assetType.IsColorAsset() && assetType != AssetType.EyeColor)
                {
                    assetId = $"{assetType}_{assetId}";
                }

                if (!assetMap.ContainsKey(assetId))
                {
                    continue;
                }
                SelectButton(assetType, assetMap[assetId]);
            }
        }
        
        public void CreateColorUI(ColorPalette[] colorPalettes, Action<object, AssetType> onClick)
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
                        UpdateSelectedAssetTypeMap(colorPalette.assetType, button);
                    }
                    assetIndex++;
                }
            }
        }

        public void SetAssetIcons(Dictionary<string, Texture> assetIcons)
        {
            foreach (var asset in assetIcons)
            {
                if (assetMap.TryGetValue(asset.Key, out AssetButton button))
                {
                    button.SetIcon(asset.Value);
                }
            }
        }

        private AssetButton AddColorButton(int index, Transform parent, AssetType assetType, Action<object, AssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(colorAssetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            var buttonName = $"{assetType}_{index}";
            assetButtonGameObject.name = buttonName;
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                UpdateSelectedAssetTypeMap(assetType, assetButton);
                assetButton.SetSelect(true);
                onClick?.Invoke(index, assetType);
            });
            assetMap.Add(buttonName, assetButton);
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
                SelectButton(assetType, assetButton);
                onClick?.Invoke(assetId, assetType);
            });
            if (assetType == AssetType.EyeColor)
            {
                assetButton.SetEyeColorConfig();
            }
            
            assetMap.Add(assetId, assetButton);
        }

        private void SelectButton(AssetType assetType, AssetButton assetButton)
        {
            UpdateSelectedAssetTypeMap(assetType, assetButton);
            assetButton.SetSelect(true);
        }

        private void UpdateSelectedAssetTypeMap(AssetType assetType, AssetButton assetButton)
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
        }

        private void AddAssetSelectionClearButton(Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetAsFirstSibling();
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(assetType, assetButton);
                onClick?.Invoke(string.Empty, assetType);
            });
            if (IsClearByDefault(assetType))
            {
                SelectButton(assetType, assetButton);
            }
        }

        private bool IsClearByDefault(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.FaceMask:
                case AssetType.Facewear:
                case AssetType.Headwear:
                case AssetType.FaceStyle:
                case AssetType.EyeShape:
                case AssetType.FaceShape:
                case AssetType.LipShape:
                case AssetType.NoseShape:
                case AssetType.Glasses:
                    return true;
                default:
                    return false;
            }
        }

    }
}
