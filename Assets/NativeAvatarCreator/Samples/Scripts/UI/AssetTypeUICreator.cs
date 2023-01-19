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
        private class AssetTypeIcon
        {
            public AssetType assetType;
            public Sprite icon;
        }

        [Serializable]
        private class AssetTypeUI
        {
            public GameObject buttonPrefab;
            public Transform buttonParent;
            public GameObject panelPrefab;
            public Transform panelParent;
        }

        [SerializeField] private AssetTypeUI assetTypeUI;
        [SerializeField] private AssetTypeButton faceAssetTypeButton;
        [SerializeField] private GameObject faceAssetTypePanel;
        [SerializeField] private GameObject faceAssetTypeParent;
        [SerializeField] private GameObject faceAssetPanelPrefab;
        [SerializeField] private List<AssetTypeIcon> assetTypeIcons;

        public Dictionary<AssetType, Transform> AssetTypePanelsMap { get; private set; }
        public Dictionary<AssetType, AssetTypeButton> AssetTypeButtonsMap { get; private set; }

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

        private void AddAssetTypeButtonsAndPanels()
        {
            AssetTypePanelsMap = new Dictionary<AssetType, Transform>();
            AssetTypeButtonsMap = new Dictionary<AssetType, AssetTypeButton>();

            foreach (var assetType in AssetTypeHelper.GetAssetTypeList())
            {
                if (AssetTypeHelper.IsFaceAsset(assetType))
                {
                    var assetTypePanel = AddAssetTypePanel(assetType, faceAssetPanelPrefab, assetTypeUI.panelParent);
                    AddAssetTypeButton(assetType, faceAssetTypeParent.transform, OnFaceTypeButton(assetTypePanel));
                }
                else
                {
                    var assetTypePanel = AddAssetTypePanel(assetType, assetTypeUI.panelPrefab, assetTypeUI.panelParent);
                    AddAssetTypeButton(assetType, assetTypeUI.buttonParent, OnAssetTypeButton(assetTypePanel));
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

        private Transform AddAssetTypePanel(AssetType assetType, GameObject panelPrefab, Transform parent)
        {
            var assetTypePanel = Instantiate(panelPrefab, parent);
            assetTypePanel.name = assetType + "Panel";
            assetTypePanel.SetActive(false);

            AssetTypePanelsMap.Add(assetType, assetTypePanel.transform);
            return assetTypePanel.transform;
        }

        private void AddAssetTypeButton(AssetType assetType, Transform parent, Action<AssetTypeButton> onClick)
        {
            var assetTypeButtonGameObject = Instantiate(assetTypeUI.buttonPrefab, parent);
            var assetTypeButton = assetTypeButtonGameObject.GetComponent<AssetTypeButton>();
            assetTypeButton.name = assetType + "Button";
            var assetTypeIcon = assetTypeIcons.FirstOrDefault(x => x.assetType == assetType);
            if (assetTypeIcon != null)
            {
                assetTypeButton.SetIcon(assetTypeIcon.icon);
            }
            assetTypeButton.AddListener(() =>
            {
                onClick?.Invoke(assetTypeButton);
            });
            AssetTypeButtonsMap.Add(assetType, assetTypeButton);
        }

        private void ResetUI()
        {
            foreach (var assetTypePanel in AssetTypePanelsMap)
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
            selectedAssetTypePanel = assetTypeUI.panelParent.GetChild(1);
            selectedAssetTypePanel.gameObject.SetActive(true);
        }
    }
}
