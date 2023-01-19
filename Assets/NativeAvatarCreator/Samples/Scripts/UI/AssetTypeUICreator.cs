using System;
using System.Collections.Generic;
using System.Linq;
using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AssetTypeUICreator : MonoBehaviour
    {
        [Serializable]
        public class AssetTypeIcon
        {
            public AssetTypeData.PartnerAssetType AssetType;
            public Sprite Icon;
        }

        [SerializeField] private GameObject assetTypePrefab;
        [SerializeField] private Transform assetTypeParent;
        [SerializeField] private GameObject assetTypePanelPrefab;
        [SerializeField] private Transform assetTypePanelParent;
        [SerializeField] private AssetTypeButton faceAssetTypeButton;
        [SerializeField] private GameObject faceAssetTypePanel;
        [SerializeField] private GameObject faceAssetTypeParent;
        [SerializeField] private GameObject faceAssetPanelPrefab;
        [SerializeField] public List<AssetTypeIcon> assetTypeIcons;

        public Dictionary<string, Transform> AssetTypesPanelsMap => assetTypesPanelsMap;

        private Dictionary<string, Transform> assetTypesPanelsMap;
        private Dictionary<string, AssetTypeButton> assetTypeButtonsMap;

        private AssetTypeButton selectedAssetTypeButton;
        private Transform selectedAssetTypePanel;

        private void Awake()
        {
            AddAssetTypeButtonsAndPanels();
        }

        private void OnDisable()
        {
            ResetUI();
        }

        public void DisableAssetTypeButton(string assetType)
        {
            var scrollRect = assetTypesPanelsMap[assetType].GetComponent<ScrollRect>();
            assetTypeButtonsMap[assetType].gameObject.SetActive(scrollRect.content.childCount != 0);
        }

        private void AddAssetTypeButtonsAndPanels()
        {
            assetTypesPanelsMap = new Dictionary<string, Transform>();
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

        private void ResetUI()
        {
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
            faceAssetTypePanel.SetActive(true);
        }

        // Selects and enables faceShape panel.   
        private void DefaultSelection()
        {
            selectedAssetTypeButton = faceAssetTypeButton;
            selectedAssetTypeButton.SetSelect(true);
            selectedAssetTypePanel = assetTypePanelParent.GetChild(1);
            selectedAssetTypePanel.gameObject.SetActive(true);
        }
    }
}
