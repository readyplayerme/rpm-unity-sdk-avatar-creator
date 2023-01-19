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
        [Serializable]
        public class AssetTypeIcon
        {
            public AssetTypeData.PartnerAssetType AssetType;
            public Sprite Icon;
        }

        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject assetTypePrefab;
        [SerializeField] private Transform assetTypeParent;
        [SerializeField] private GameObject assetTypePanelPrefab;
        [SerializeField] private Transform assetTypePanelParent;
        [SerializeField] private GameObject clearAssetSelectionButton;
        [SerializeField] private AssetTypeButton faceAssetTypeButton;
        [SerializeField] private GameObject faceAssetTypePanel;
        [SerializeField] private GameObject faceAssetTypeParent;
        [SerializeField] private GameObject faceAssetPanelPrefab;
        [SerializeField] public List<AssetTypeIcon> assetTypeIcons;

        public Action Show;
        public Action Hide;

        private Dictionary<string, Transform> assetTypesPanelsMap;
        private Dictionary<string, AssetButton> selectedAssetByTypeMap;
        private Dictionary<string, AssetTypeButton> assetTypeButtonsMap;

        private AssetTypeButton selectedAssetTypeButton;
        private Transform selectedAssetTypePanel;

        private void Awake()
        {
            AddAssetTypeButtonsAndPanels();
        }

        private void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }

        private void OnDisable()
        {
            Hide?.Invoke();
            foreach (var assetTypePanel in assetTypesPanelsMap)
            {
                var parent = assetTypePanel.Value.GetComponent<ScrollRect>().content;
                foreach (Transform assetButton in parent)
                {
                    Destroy(assetButton.gameObject);
                }
                assetTypePanel.Value.gameObject.SetActive(false);
            }
            DefaultSelection();

        }

        public void InstantNoodles(Dictionary<PartnerAsset, Task<Texture>> assets, Action<string, string> onClick)
        {
            foreach (var asset in assets)
            {
                AddAssetButton(asset.Key.Id, asset.Key.AssetType, onClick, asset.Value);
            }

            foreach (var assetType in assetTypesPanelsMap)
            {
                if (assetType.Key != "outfit" && assetType.Key != "shirt")
                {
                    AddAssetSelectionClearButton(assetType.Value, assetType.Key, onClick);
                }

                // Disable the asset type which doesn't have any assets.
                assetTypeButtonsMap[assetType.Key].gameObject.SetActive(assetType.Value.GetComponent<ScrollRect>().content.childCount != 0);
            }

            faceAssetTypePanel.SetActive(true);
            Loading.SetActive(false);
        }

        private void AddAssetTypeButtonsAndPanels()
        {
            assetTypesPanelsMap = new Dictionary<string, Transform>();
            selectedAssetByTypeMap = new Dictionary<string, AssetButton>();
            assetTypeButtonsMap = new Dictionary<string, AssetTypeButton>();

            foreach (var assetType in AssetTypeData.PartnerAssetTypeMap.Keys)
            {
                if (AssetTypeData.IsFaceAsset(assetType))
                {
                    var assetTypePanel = AddAssetTypePanel(assetType, faceAssetPanelPrefab, assetTypePanelParent);
                    AddAssetTypeButton(assetType, faceAssetTypeParent.transform, OnFaceTypeButton(assetTypePanel));
                }
                else
                {
                    var assetTypePanel = AddAssetTypePanel(assetType, assetTypePanelPrefab, assetTypePanelParent);
                    AddAssetTypeButton(assetType, assetTypeParent, OnAssetTypeButton(assetTypePanel));
                }
            }

            DefaultSelection();

            faceAssetTypeButton.AddListener(() =>
            {
                if (selectedAssetTypeButton != null)
                {
                    selectedAssetTypeButton.SetSelect(false);
                }

                selectedAssetTypePanel.gameObject.SetActive(false);
                DefaultSelection();
                faceAssetTypePanel.SetActive(true);
            });
        }

        private Action<AssetTypeButton> OnAssetTypeButton(Transform assetTypePanel)
        {
            return assetTypeButton =>
            {
                selectedAssetTypeButton.SetSelect(false);
                assetTypeButton.SetSelect(true);
                selectedAssetTypeButton = assetTypeButton;

                faceAssetTypePanel.SetActive(false);
                selectedAssetTypePanel.gameObject.SetActive(false);
                faceAssetTypeButton.SetSelect(false);
                assetTypePanel.gameObject.SetActive(true);
                selectedAssetTypePanel = assetTypePanel.transform;
            };
        }

        private Action<AssetTypeButton> OnFaceTypeButton(Transform assetTypePanel)
        {
            return assetTypeButton =>
            {
                faceAssetTypePanel.SetActive(true);
                if (selectedAssetTypePanel != faceAssetTypePanel.transform)
                {
                    selectedAssetTypePanel.gameObject.SetActive(false);
                }
                        
                if (selectedAssetTypeButton != faceAssetTypeButton)
                {
                    selectedAssetTypeButton.SetSelect(false);
                }
                assetTypeButton.SetSelect(true);
                selectedAssetTypeButton = assetTypeButton;

                assetTypePanel.gameObject.SetActive(true);
                selectedAssetTypePanel = assetTypePanel.transform;
            };
        }

        private Transform AddAssetTypePanel(string assetType, GameObject panelPrefab, Transform parent)
        {
            var assetTypePanel = Instantiate(panelPrefab, parent);
            assetTypePanel.name = assetType + "Panel";
            assetTypePanel.SetActive(false);

            assetTypesPanelsMap.Add(assetType, assetTypePanel.transform);
            return assetTypePanel.transform;
        }

        private void AddAssetTypeButton(string assetType, Transform parent, Action<AssetTypeButton> onClick)
        {
            var assetTypeButtonGameObject = Instantiate(assetTypePrefab, parent);
            var assetTypeButton = assetTypeButtonGameObject.GetComponent<AssetTypeButton>();
            assetTypeButton.name = assetType + "Button";
            var assetTypeIcon = assetTypeIcons.FirstOrDefault(x => x.AssetType == AssetTypeData.PartnerAssetTypeMap[assetType]);
            if (assetTypeIcon != null)
            {
                assetTypeButton.SetIcon(assetTypeIcon.Icon);
            }
            assetTypeButton.AddListener(() =>
            {
                onClick?.Invoke(assetTypeButton);
            });
            assetTypeButtonsMap.Add(assetType, assetTypeButton);
        }

        private void AddAssetSelectionClearButton(Transform parent, string assetType, Action<string, string> onClick)
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

        private async void AddAssetButton(string assetId, string assetType, Action<string, string> onClick,
            Task<Texture> iconDownloadTask)
        {
            var parent = assetTypesPanelsMap[assetType];

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

        // Selects and enabled faceShape panel.   
        private void DefaultSelection()
        {
            selectedAssetTypeButton = faceAssetTypeButton;
            selectedAssetTypeButton.SetSelect(true);
            selectedAssetTypePanel = assetTypePanelParent.GetChild(1);
            selectedAssetTypePanel.gameObject.SetActive(true);
        }
    }
}
