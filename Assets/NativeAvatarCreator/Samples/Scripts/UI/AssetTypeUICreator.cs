using System;
using System.Collections.Generic;
using System.Linq;
using NativeAvatarCreator;
using UnityEngine;

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
        [SerializeField] private GameObject eyeColorPanel;
        [SerializeField] private CameraZoom cameraZoom;
        [SerializeField] private List<AssetTypeIcon> assetTypeIcons;

        public Dictionary<AssetType, Transform> AssetTypePanelsMap { get; private set; }

        private Dictionary<AssetType, AssetTypeButton> assetTypeButtonsMap;
        private AssetTypeButton selectedAssetTypeButton;
        private Transform selectedAssetTypePanel;

        public void CreateUI(IEnumerable<AssetType> assetTypes)
        {
            AssetTypePanelsMap = new Dictionary<AssetType, Transform>();
            assetTypeButtonsMap = new Dictionary<AssetType, AssetTypeButton>();

            foreach (var assetType in assetTypes)
            {
                if (assetType == AssetType.EyeColor)
                {
                    continue;
                }
                
                if (AssetTypeHelper.IsFaceAsset(assetType))
                {
                    var assetTypePanel = CreateAssetTypePanel(assetType, faceAssetPanelPrefab, assetTypeUI.panelParent);
                    CreateAssetTypeButton(assetType, faceAssetTypeParent.transform, OnFaceTypeButton(assetType, assetTypePanel));
                }
                else
                {
                    var assetTypePanel = CreateAssetTypePanel(assetType, assetTypeUI.panelPrefab, assetTypeUI.panelParent);
                    CreateAssetTypeButton(assetType, assetTypeUI.buttonParent, OnAssetTypeButton(assetType, assetTypePanel));
                }
            }

            DefaultSelection();
            AssetTypePanelsMap.Add(AssetType.EyeColor, eyeColorPanel.transform);
            
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

        public void ResetUI()
        {
            foreach (var assetTypePanel in AssetTypePanelsMap)
            {
                var parent = assetTypePanel.Value;
                Destroy(parent.gameObject);
                assetTypePanel.Value.gameObject.SetActive(false);
            }
            AssetTypePanelsMap.Clear();

            foreach (var assetTypeButton in assetTypeButtonsMap)
            {
                Destroy(assetTypeButton.Value.gameObject);
            }
            faceAssetTypeButton.RemoveListener();
            assetTypeButtonsMap.Clear();
        }

        private Action<AssetTypeButton> OnAssetTypeButton(AssetType assetType, Transform assetTypePanel)
            => assetTypeButton =>
            {
                if (assetType == AssetType.Outfit)
                {
                    cameraZoom.MoveToFar();
                }
                else
                {
                    cameraZoom.MoveToNear();
                }

                selectedAssetTypeButton.SetSelect(false);
                assetTypeButton.SetSelect(true);
                selectedAssetTypeButton = assetTypeButton;
                eyeColorPanel.SetActive(false);
                faceAssetTypePanel.SetActive(false);
                selectedAssetTypePanel.gameObject.SetActive(false);
                faceAssetTypeButton.SetSelect(false);
                assetTypePanel.gameObject.SetActive(true);
                selectedAssetTypePanel = assetTypePanel.transform;
            };

        private Action<AssetTypeButton> OnFaceTypeButton(AssetType assetType, Transform assetTypePanel)
        {
            return assetTypeButton =>
            {
                cameraZoom.MoveToNear();
                faceAssetTypePanel.SetActive(true);
                if (selectedAssetTypePanel != faceAssetTypePanel.transform)
                {
                    selectedAssetTypePanel.gameObject.SetActive(false);
                }

                if (selectedAssetTypeButton != faceAssetTypeButton)
                {
                    selectedAssetTypeButton.SetSelect(false);
                }

                eyeColorPanel.SetActive(assetType == AssetType.EyeShape);

                assetTypeButton.SetSelect(true);
                selectedAssetTypeButton = assetTypeButton;

                assetTypePanel.gameObject.SetActive(true);
                selectedAssetTypePanel = assetTypePanel.transform;
            };
        }

        private Transform CreateAssetTypePanel(AssetType assetType, GameObject panelPrefab, Transform parent)
        {
            var assetTypePanel = Instantiate(panelPrefab, parent);
            assetTypePanel.name = assetType + "Panel";
            assetTypePanel.SetActive(false);

            AssetTypePanelsMap.Add(assetType, assetTypePanel.transform);
            return assetTypePanel.transform;
        }

        private void CreateAssetTypeButton(AssetType assetType, Transform parent, Action<AssetTypeButton> onClick)
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
            assetTypeButtonsMap.Add(assetType, assetTypeButton);
        }

        // Selects and enables faceShape panel.   
        private void DefaultSelection()
        {
            selectedAssetTypeButton = faceAssetTypeButton;
            selectedAssetTypeButton.SetSelect(true);
            selectedAssetTypePanel = assetTypeUI.panelParent.GetChild(2);
            selectedAssetTypePanel.gameObject.SetActive(true);
        }
    }
}
