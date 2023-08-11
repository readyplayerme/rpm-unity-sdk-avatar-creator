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

        private Dictionary<object, AssetButton> buttonsById;
        private Dictionary<AssetType, AssetButton> selectedButtonsByCategory;
        private Dictionary<AssetType, object> selectedAssets;

        public void SetSelectedAssets(Dictionary<AssetType, object> assets)
        {
            selectedAssets = assets;
            selectedButtonsByCategory = new Dictionary<AssetType, AssetButton>();
        }

        public void CreateAssetButtons(IEnumerable<string> assets, AssetType assetType, Action<string, AssetType> onClick)
        {
            selectedButtonsByCategory = new Dictionary<AssetType, AssetButton>();
            buttonsById = new Dictionary<object, AssetButton>();

            var parentPanel = PanelSwitcher.AssetTypePanelMap[assetType];
            foreach (var asset in assets)
            {
                AddAssetButton(asset, parentPanel.transform, assetType, onClick);
                if (selectedAssets.ContainsValue(asset))
                {
                    SetSelectedIcon(asset, assetType);
                }
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
                    AddClearSelectionButton(assetTypePanel.transform, assetType, onClick);
                }
            }
        }

        private void SetSelectedIcon(string assetId, AssetType assetType)
        {
            if (assetType.IsColorAsset() && assetType != AssetType.EyeColor)
            {
                assetId = $"{assetType}_{assetId}";
            }

            if (!buttonsById.ContainsKey(assetId))
            {
                return;
            }
            SelectButton(assetType, buttonsById[assetId]);
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
                        SelectButton(colorPalette.assetType, button);
                    }
                    assetIndex++;
                }
            }
        }

        public void SetAssetIcon(string id, Texture texture)
        {
            if (buttonsById.TryGetValue(id, out AssetButton button))
            {
                button.SetIcon(texture);
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
                SelectButton(assetType, assetButton);
                onClick?.Invoke(index, assetType);
            });
            buttonsById.Add(buttonName, assetButton);
            return assetButton;
        }

        private void AddAssetButton(string assetId, Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            if (buttonsById.ContainsKey(assetId)) return;

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
            buttonsById.Add(assetId, assetButton);
        }
        
        private void SelectButton(AssetType assetType, AssetButton assetButton)
        {
            if (selectedButtonsByCategory.ContainsKey(assetType))
            {
                selectedButtonsByCategory[assetType].SetSelect(false);
                selectedButtonsByCategory[assetType] = assetButton;
            }
            else
            {
                selectedButtonsByCategory.Add(assetType, assetButton);
            }
            assetButton.SetSelect(true);
        }

        private void AddClearSelectionButton(Transform parent, AssetType assetType, Action<string, AssetType> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetAsFirstSibling();
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(assetType, assetButton);
                onClick?.Invoke(string.Empty, assetType);
            });

            if (IsSelectedAssetNotPresentForAssetType(assetType))
            {
                SelectButton(assetType, assetButton);
            }
            
        }

        private bool IsSelectedAssetNotPresentForAssetType(AssetType assetType)
        {
            return !selectedAssets.ContainsKey(assetType) || 
                   selectedAssets[assetType] is int intValue && intValue == 0 || 
                   selectedAssets[assetType] is string strValue && string.IsNullOrEmpty(strValue);
        }
    }
}
