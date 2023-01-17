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
        [SerializeField] private GameObject assetTypePrefab;
        [SerializeField] private Transform assetTypeParent;
        [SerializeField] private GameObject assetTypePanelPrefab;
        [SerializeField] private Transform assetTypePanelParent;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private GameObject faceAssetTypeButton;
        [SerializeField] private GameObject faceAssetTypePanel;
        [SerializeField] private GameObject faceAssetTypeParent;
        [SerializeField] private GameObject faceAssetPanelPrefab;

        public Action Show;
        public Action Hide;

        private Dictionary<string, Transform> assetTypesPanels;
        private Dictionary<string, AssetButton> selectedAssetByType;

        private Transform selectedAssetType;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        private void OnDisable()
        {
            Hide?.Invoke();
            foreach (var assetTypePanel in assetTypesPanels)
            {
                var parent = assetTypePanel.Value.GetComponent<ScrollRect>().content;
                foreach (Transform assetButton in parent)
                {
                    Destroy(assetButton.gameObject);
                }
            }
        }

        private void Initialize()
        {
            assetTypesPanels = new Dictionary<string, Transform>();
            selectedAssetByType = new Dictionary<string, AssetButton>();

            foreach (var assetType in AssetType.Face)
            {
                var assetTypePanel = AddAssetTypePanel(assetType, faceAssetPanelPrefab, assetTypePanelParent);

                AddAssetTypeButton(assetType, faceAssetTypeParent.transform, () =>
                {
                    faceAssetTypePanel.SetActive(true);
                    selectedAssetType.gameObject.SetActive(false);
                    assetTypePanel.gameObject.SetActive(true);
                    selectedAssetType = assetTypePanel.transform;
                });
            }

            foreach (var assetType in AssetType.Body)
            {
                var assetTypePanel = AddAssetTypePanel(assetType, assetTypePanelPrefab, assetTypePanelParent);

                AddAssetTypeButton(assetType, assetTypeParent, () =>
                {
                    faceAssetTypePanel.SetActive(false);
                    selectedAssetType.gameObject.SetActive(false);
                    assetTypePanel.gameObject.SetActive(true);
                    selectedAssetType = assetTypePanel.transform;
                });
            }

            selectedAssetType = assetTypePanelParent.GetChild(1);
            selectedAssetType.gameObject.SetActive(true);

            faceAssetTypeButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedAssetType.gameObject.SetActive(false);
                selectedAssetType = assetTypePanelParent.GetChild(1);
                selectedAssetType.gameObject.SetActive(true);
                faceAssetTypePanel.SetActive(true);
            });
        }

        public void InstantNoodles(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, string> onClick)
        {
            foreach (var assetType in assetTypesPanels)
            {
                if (assetType.Key != "outfit")
                {
                    AddAssetSelectionClearButton(assetType.Value, assetType.Key, onClick);
                }
            }

            foreach (var asset in assets)
            {
                AddAssetButton(asset.Key.Id, asset.Key.AssetType, onClick, asset.Value);
            }

            faceAssetTypePanel.SetActive(true);
            Loading.SetActive(false);
        }

        private Transform AddAssetTypePanel(string assetType, GameObject panelPrefab, Transform parent)
        {
            var assetTypePanel = Instantiate(panelPrefab, parent);
            assetTypePanel.name = assetType + "Panel";
            assetTypePanel.SetActive(false);

            assetTypesPanels.Add(assetType, assetTypePanel.transform);
            return assetTypePanel.transform;
        }

        private void AddAssetTypeButton(string assetType, Transform parent, Action onClick)
        {
            var assetTypeButton = Instantiate(assetTypePrefab, parent);
            assetTypeButton.name = assetType + "Button";
            assetTypeButton.GetComponent<Button>().onClick.AddListener(onClick.Invoke);
        }

        private void AddAssetSelectionClearButton(Transform parent, string assetType, Action<string, string> onClick)
        {
            var assetButtonGameObject = Instantiate(clearAssetSelectionButton, parent.GetComponent<ScrollRect>().content);
            assetButtonGameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedAssetByType.ContainsKey(assetType))
                {
                    selectedAssetByType[assetType].SetSelect(false);
                }

                onClick?.Invoke(string.Empty, assetType);
            });
        }

        private async void AddAssetButton(string assetId, string assetType, Action<string, string> onClick,
            Task<Texture> iconDownloadTask)
        {
            var parent = assetTypesPanels[assetType];

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
