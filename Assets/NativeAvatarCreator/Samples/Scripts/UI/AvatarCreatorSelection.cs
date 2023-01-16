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
        [SerializeField] private GameObject clearAssetSelectionButton;

        public Action Show;
        public Action Hide;

        private Dictionary<string, Transform> assetTypes;
        private Dictionary<string, AssetButton> selectedAssetByType = new Dictionary<string, AssetButton>();

        private List<Transform> assetTypePanels;
        private Transform selectedAssetType;

        public void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        public void OnDisable()
        {
            Hide?.Invoke();
            foreach (var assetTypePanel in assetTypePanels)
            {
                foreach (Transform assetButton in assetTypePanel)
                {
                    Destroy(assetButton.gameObject);
                }
                Destroy(assetTypePanel.gameObject);
            }

            foreach (Transform assetTypeButton in assetTypeParent)
            {
                Destroy(assetTypeButton.gameObject);
            }
        }

        public void InstantNoodles(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, string> onClick)
        {
            assetTypes = new Dictionary<string, Transform>();
            assetTypePanels = new List<Transform>();
            selectedAssetByType = new Dictionary<string, AssetButton>();

            foreach (var asset in assets)
            {
                var assetType = asset.Key.AssetType;
                var parent = AddAssetTypeButtonAndPanel(assetType);
                AddAssetButton(parent, asset.Key.Id, assetType, onClick, asset.Value);
            }

            foreach (var assetType in assetTypes)
            {
                if (assetType.Key != "outfit")
                {
                    AddAssetSelectionClearButton(assetType.Value, assetType.Key, onClick);
                }
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

                assetTypePanels.Add(assetTypePanel.transform);

                assetTypeButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedAssetType.gameObject.SetActive(false);
                    assetTypePanel.SetActive(true);
                    selectedAssetType = assetTypePanel.transform;
                });
            }

            return parent;
        }

        private void AddAssetSelectionClearButton(Transform parent, string assetType, Action<string, string> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.transform.SetSiblingIndex(0);
            assetButtonGameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedAssetByType.ContainsKey(assetType))
                {
                    selectedAssetByType[assetType].SetSelect(false);
                }

                onClick?.Invoke(string.Empty, assetType);
            });
        }


        private async void AddAssetButton(Transform parent, string assetId, string assetType, Action<string, string> onClick,
            Task<Texture> iconDownloadTask)
        {
            var assetButtonGameObject = Instantiate(assetButtonPrefab, parent.GetComponent<ScrollRect>().content);
            var assetButton = assetButtonGameObject.GetComponent<AssetButton>();
            assetButton.AddListener(() =>
            {
                if (selectedAssetByType.ContainsKey(assetType))
                {
                    selectedAssetByType[assetType].SetSelect(false);
                    selectedAssetByType[assetType] = assetButton;
                }
                else
                {
                    selectedAssetByType.Add(assetType, assetButton);
                }

                assetButton.SetSelect(true);
                onClick?.Invoke(assetId, assetType);
            });
            assetButton.SetIcon(await iconDownloadTask);
        }
    }
}
