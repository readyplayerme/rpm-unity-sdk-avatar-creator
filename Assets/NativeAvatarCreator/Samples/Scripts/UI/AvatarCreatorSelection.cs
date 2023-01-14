using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AvatarCreatorSelection : SelectionPanel
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject assetTypePrefab;
        [SerializeField] private Transform assetTypeParent;
        [SerializeField] private GameObject assetTypePanelPrefab;
        [SerializeField] private Transform assetTypePanelParent;

        public Action Show;

        private Dictionary<string, Transform> assetTypes;

        private Transform selectedAssetType;

        public void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        public void InstantNoodles(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, string> onClick)
        {
            assetTypes = new Dictionary<string, Transform>();

            foreach (var asset in assets)
            {
                var assetType = asset.Key.AssetType;
                var parent = AddAssetTypeButtonAndPanel(assetType);
                AddAssetButton(parent, asset.Key.Id, assetType, onClick, asset.Value);
            }

            selectedAssetType = assetTypes.First().Value;
            selectedAssetType.gameObject.SetActive(true);

            Loading.SetActive(false);
        }

        private Transform AddAssetTypeButtonAndPanel(string assetType)
        {
            Transform parent;
            if (assetTypes.ContainsKey(assetType))
            {
                parent = assetTypes[assetType];
            }
            else
            {
                var assetTypeButton = Instantiate(assetTypePrefab, assetTypeParent);
                assetTypeButton.GetComponentInChildren<Text>().text = assetType;
                assetTypeButton.name = assetType + "Button";

                var assetTypePanel = Instantiate(assetTypePanelPrefab, assetTypePanelParent);
                assetTypePanel.name = assetType + "Panel";

                assetTypes.Add(assetType, assetTypePanel.transform);
                assetTypePanel.SetActive(false);
                parent = assetTypePanel.transform;

                assetTypeButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedAssetType.gameObject.SetActive(false);
                    assetTypePanel.SetActive(true);
                    selectedAssetType = assetTypePanel.transform;
                });
            }

            return parent;
        }

        private async void AddAssetButton(Transform parent, string assetId, string assetType, Action<string, string> onClick,
            Task<Texture> iconDownloadTask)
        {
            var assetButton = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            assetButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClick?.Invoke(assetId, assetType);
            });
            assetButton.GetComponent<RawImage>().texture = await iconDownloadTask;
        }
    }
}
