using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class CategoryUICreator : MonoBehaviour
    {
        private const string PANEL_SUFFIX = "Panel";
        private const string BUTTON_SUFFIX = "Button";
        
        [Serializable]
        private class CategoryIcon
        {
            public Category category;
            public Sprite icon;
        }

        [Serializable]
        private class CategoryUI
        {
            public GameObject buttonPrefab;
            public Transform buttonParent;
            public GameObject panelPrefab;
            public Transform panelParent;
        }

        [SerializeField] private CategoryUI categoryUI;
        [SerializeField] private CategoryButton faceCategoryButton;
        [SerializeField] private GameObject faceCategoryPanel;
        [SerializeField] private GameObject faceAssetPanelPrefab;
        [SerializeField] private GameObject leftSidePanelPrefab;
        [SerializeField] private List<CategoryIcon> categoryIcons;

        private Dictionary<Category, CategoryButton> categoryButtonsMap;
        public Action<Category> OnCategorySelected;
        private CategoryButton selectedCategoryButton;

        private CameraZoom cameraZoom;
        private BodyType bodyType;
        
        private void Awake()
        {
            cameraZoom = FindObjectOfType<CameraZoom>();
        }

        public void CreateUI(BodyType bodyType, IEnumerable<Category> categories)
        {
            this.bodyType = bodyType;
            DefaultZoom();

            categoryButtonsMap = new Dictionary<Category, CategoryButton>();
            PanelSwitcher.FaceTypePanel = faceCategoryPanel;
            CreateCategoryPanel(Category.SkinColor, leftSidePanelPrefab, categoryUI.panelParent);
            foreach (var category in categories)
            {
                if (category.IsColorAsset())
                {
                    CreateCategoryPanel(category, leftSidePanelPrefab, categoryUI.panelParent);
                }
                else if (category.IsFaceAsset())
                {
                    CreateCategoryPanel(category, faceAssetPanelPrefab, categoryUI.panelParent);
                    CreateCategoryButton(category, faceCategoryPanel.GetComponent<ScrollRect>().content.transform);
                }
                else
                {
                    CreateCategoryPanel(category, categoryUI.panelPrefab, categoryUI.panelParent);
                    CreateCategoryButton(category, categoryUI.buttonParent);
                }
            }

            DefaultSelection();
            faceCategoryButton.AddListener(() =>
            {
                if (selectedCategoryButton != null)
                {
                    selectedCategoryButton.SetSelect(false);
                }

                DefaultSelection();
            });
        }

        public void SetDefaultSelection(Category category)
        {
            SwitchZoomByCategory(category);
            categoryButtonsMap[category].SetSelect(true);
            selectedCategoryButton.SetSelect(false);
            faceCategoryButton.SetSelect(category.IsFaceAsset());
            selectedCategoryButton = categoryButtonsMap[category];
            PanelSwitcher.Switch(category);
        }
        
        public void SetActiveCategoryButtons(bool enable)
        {
            faceCategoryButton.SetInteractable(enable);
            foreach (var categoryButton in categoryButtonsMap)
            {
                if (categoryButton.Key != Category.Outfit)
                {
                    categoryButton.Value.SetInteractable(enable);
                }
            }
        }

        public void ResetUI()
        {
            PanelSwitcher.Clear();
            DefaultZoom();

            if (categoryButtonsMap == null)
            {
                return;
            }

            foreach (var categoryButton in categoryButtonsMap)
            {
                Destroy(categoryButton.Value.gameObject);
            }

            faceCategoryButton.RemoveListener();
            categoryButtonsMap.Clear();
        }

        private void CreateCategoryPanel(Category category, GameObject panelPrefab, Transform parent)
        {
            var categoryPanel = Instantiate(panelPrefab, parent);
            categoryPanel.name = category + PANEL_SUFFIX;
            categoryPanel.SetActive(false);

            PanelSwitcher.AddPanel(category, categoryPanel);
        }

        private void CreateCategoryButton(Category category, Transform parent)
        {
            var categoryButtonGameObject = Instantiate(categoryUI.buttonPrefab, parent);
            var categoryButton = categoryButtonGameObject.GetComponent<CategoryButton>();
            categoryButton.name = category + BUTTON_SUFFIX;
            var categoryIcon = categoryIcons.FirstOrDefault(x => x.category == category);
            if (categoryIcon != null)
            {
                categoryButton.SetIcon(categoryIcon.icon);
            }

            categoryButton.AddListener(() =>
            {
                SetDefaultSelection(category);
                OnCategorySelected?.Invoke(category);
            });
            categoryButtonsMap.Add(category, categoryButton);
        }

        private void DefaultSelection()
        {
            faceCategoryButton.SetSelect(true);
            categoryButtonsMap[Category.FaceShape].SetSelect(true);
            PanelSwitcher.Switch(Category.FaceShape);
            selectedCategoryButton = categoryButtonsMap[Category.FaceShape];
        }

        private void DefaultZoom()
        {
            if (bodyType == BodyType.HalfBody)
            {
                cameraZoom.MoveToHalfBody();
            }
            else
            {
                cameraZoom.MoveToFar();
            }
        }

        private void SwitchZoomByCategory(Category category)
        {
            if (bodyType != BodyType.HalfBody)
            {
                if (category == Category.Outfit)
                {
                    cameraZoom.MoveToFar();
                }
                else
                {
                    cameraZoom.MoveToNear();
                }
            }
        }
    }
}
