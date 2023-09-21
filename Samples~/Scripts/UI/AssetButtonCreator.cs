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
        private Dictionary<Category, AssetButton> selectedButtonsByCategory;
        private Dictionary<Category, object> selectedAssets;

        private void Start()
        {
            buttonsById = new Dictionary<object, AssetButton>();
            selectedButtonsByCategory = new Dictionary<Category, AssetButton>();
        }

        private void OnDisable()
        {
            selectedButtonsByCategory.Clear();
        }

        public void SetSelectedAssets(Dictionary<Category, object> assets)
        {
            selectedAssets = assets;
        }

        public void CreateAssetButtons(IEnumerable<string> assets, Category category, Action<string, Category> onClick)
        {
            buttonsById = new Dictionary<object, AssetButton>();

            var parentPanel = PanelSwitcher.CategoryPanelMap[category];
            foreach (var asset in assets)
            {
                AddAssetButton(asset, parentPanel.transform, category, onClick);
                if (selectedAssets.ContainsValue(asset))
                {
                    SetSelectedIcon(asset, category);
                }
            }
        }

        public void CreateClearButton(Action<string, Category> onClick)
        {
            foreach (var categoryPanelMap in PanelSwitcher.CategoryPanelMap)
            {
                var category = categoryPanelMap.Key;
                if (category.IsOptionalAsset())
                {
                    var categoryPanel = categoryPanelMap.Value;
                    AddClearSelectionButton(categoryPanel.transform, category, onClick);
                }
            }
        }

        private void SetSelectedIcon(string assetId, Category category)
        {
            if (category.IsColorAsset() && category != Category.EyeColor)
            {
                assetId = $"{category}_{assetId}";
            }

            if (!buttonsById.ContainsKey(assetId))
            {
                return;
            }
            SelectButton(category, buttonsById[assetId]);
        }

        public void CreateColorUI(ColorPalette[] colorPalettes, Action<object, Category> onClick)
        {
            foreach (var colorPalette in colorPalettes)
            {
                var parent = PanelSwitcher.CategoryPanelMap[colorPalette.category];
                var assetIndex = 0;
                foreach (var color in colorPalette.hexColors)
                {
                    var button = AddColorButton(assetIndex, parent.transform, colorPalette.category, onClick);
                    button.SetColor(color);

                    // By default first color is applied on initial draft
                    if (assetIndex == 0)
                    {
                        SelectButton(colorPalette.category, button);
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

        private AssetButton AddColorButton(int index, Transform parent, Category category, Action<object, Category> onClick)
        {
            var assetButtonGameObject = Instantiate(colorAssetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            var buttonName = $"{category}_{index}";
            assetButtonGameObject.name = buttonName;
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(category, assetButton);
                onClick?.Invoke(index, category);
            });
            buttonsById.Add(buttonName, assetButton);
            return assetButton;
        }

        private void AddAssetButton(string assetId, Transform parent, Category category, Action<string, Category> onClick)
        {
            if (buttonsById.ContainsKey(assetId)) return;

            var assetButtonGameObject = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.name = "Asset-" + assetId;
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(category, assetButton);
                onClick?.Invoke(assetId, category);
            });
            if (category == Category.EyeColor)
            {
                assetButton.SetEyeColorConfig();
            }
            buttonsById.Add(assetId, assetButton);
        }
        
        private void SelectButton(Category category, AssetButton assetButton)
        {
            ConfigureOutfitSelection(category);
            
            if (selectedButtonsByCategory.ContainsKey(category))
            {
                selectedButtonsByCategory[category].SetSelect(false);
                selectedButtonsByCategory[category] = assetButton;
            }
            else
            {
                selectedButtonsByCategory.Add(category, assetButton);
            }
            assetButton.SetSelect(true);
        }

        private void ConfigureOutfitSelection(Category category)
        {
            switch (category)
            {
                case Category.Top:
                case Category.Bottom:
                case Category.Footwear:
                {
                    if (selectedButtonsByCategory.TryGetValue(Category.Outfit, out AssetButton outfitButton))
                    {
                        outfitButton.SetSelect(false);
                    }
                    break;
                }
                case Category.Outfit:
                {
                    if (selectedButtonsByCategory.TryGetValue(Category.Top, out AssetButton topButton))
                    {
                        topButton.SetSelect(false);
                    }
                
                    if (selectedButtonsByCategory.TryGetValue(Category.Bottom, out AssetButton bottomButton))
                    {
                        bottomButton.SetSelect(false);
                    }
                
                    if (selectedButtonsByCategory.TryGetValue(Category.Footwear, out AssetButton footwearButton))
                    {
                        footwearButton.SetSelect(false);
                    }
                    break;
                }
            }
        }

        private void AddClearSelectionButton(Transform parent, Category category, Action<string, Category> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetAsFirstSibling();
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                SelectButton(category, assetButton);
                onClick?.Invoke(string.Empty, category);
            });

            if (IsSelectedAssetNotPresentForCategory(category))
            {
                SelectButton(category, assetButton);
            }
            
        }

        private bool IsSelectedAssetNotPresentForCategory(Category category)
        {
            return !selectedAssets.ContainsKey(category) ||
                   selectedAssets[category] as int? == 0 ||
                   string.IsNullOrEmpty(selectedAssets[category] as string);
        }
    }
}
