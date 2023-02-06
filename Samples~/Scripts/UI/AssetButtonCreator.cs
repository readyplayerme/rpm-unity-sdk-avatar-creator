using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AssetButtonCreator : MonoBehaviour
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject clearAssetSelectionButton;

        private Dictionary<AssetType, AssetButton> selectedAssetByTypeMap;

        public void CreateUI(IEnumerable<KeyValuePair<PartnerAsset, Task<Texture>>> assets, Action<string, AssetType> onClick)
        {
            selectedAssetByTypeMap = new Dictionary<AssetType, AssetButton>();

            foreach (var asset in assets)
            {
                var parent = PanelSwitcher.AssetTypePanelMap[asset.Key.AssetType];
                AddAssetButton(asset.Key.Id, parent.transform, asset.Key.AssetType, onClick, asset.Value);
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
            assetButton.SetIcon(assetType, await iconDownloadTask);
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
