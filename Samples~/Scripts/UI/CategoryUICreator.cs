using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class CategoryUICreator : MonoBehaviour
    {
        [Serializable]
        private class CategoryIcon
        {
            public Category category;
            public GameObject panelParent;
        }

        [SerializeField] private CategoryButton faceCategoryButton;
        [SerializeField] private GameObject faceCategoryPanel;
        [SerializeField] private CategoryButton outfitCategoryButton;
        [SerializeField] private GameObject outfitCategoryPanel;
        [SerializeField] private List<CategoryButton> categoryButtons;
        [SerializeField] private List<CategoryIcon> categoryPanels;

        private Dictionary<Category, CategoryButton> categoryButtonsMap;
        public Action<Category> OnCategorySelected;
        private CategoryButton selectedCategoryButton;

        private CameraZoom cameraZoom;
        private BodyType bodyType;

        private void Awake()
        {
            cameraZoom = FindObjectOfType<CameraZoom>();
            Initialize();
        }

        private void OnEnable()
        {
            faceCategoryButton.AddListener(SelectFaceShapeCategory);
            outfitCategoryButton.AddListener(SelectOutfitTopCategory);
        }

        private void OnDisable()
        {
            faceCategoryButton.RemoveListener();
            outfitCategoryButton.RemoveListener();
        }

        private void Initialize()
        {
            categoryButtonsMap = new Dictionary<Category, CategoryButton>();
            PanelSwitcher.FaceCategoryPanel = faceCategoryPanel;
            PanelSwitcher.OutfitCategoryPanel = outfitCategoryPanel;

            foreach (CategoryButton categoryButton in categoryButtons)
            {
                ConfigureCategoryButton(categoryButton.Category, categoryButton);
            }

            foreach (var category in categoryPanels)
            {
                PanelSwitcher.AddPanel(category.category, category.panelParent);
            }
        }
        
        public void Setup(BodyType bodyType)
        { 
            this.bodyType = bodyType;
            DefaultZoom();

            if (this.bodyType == BodyType.HalfBody)
            {
                categoryButtons.First(x => x.Category == Category.Outfit).gameObject.SetActive(false);
                categoryButtons.First(x => x.Category == Category.Shirt).gameObject.SetActive(true);
            }
            else
            {
                categoryButtons.First(x => x.Category == Category.Shirt).gameObject.SetActive(false);
                categoryButtons.First(x => x.Category == Category.Outfit).gameObject.SetActive(true);
            }

            foreach (var category in categoryPanels)
            {
                category.panelParent.SetActive(false);
            }

            SelectFaceShapeCategory();
        }

        public void SetDefaultSelection(Category category)
        {
            SwitchZoomByCategory(category);
            categoryButtonsMap[category].SetSelect(true);
            selectedCategoryButton.SetSelect(false);
            faceCategoryButton.SetSelect(category.IsFaceAsset());
            outfitCategoryButton.SetSelect(category.IsOutfitAsset());
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

            foreach (var categoryButton in categoryButtons)
            {
                categoryButton.SetSelect(false);
            }
        }

        private void ConfigureCategoryButton(Category category, CategoryButton categoryButton)
        {
            categoryButton.AddListener(() =>
            {
                SetDefaultSelection(category);
                OnCategorySelected?.Invoke(category);
            });

            categoryButtonsMap.Add(category, categoryButton);
        }
        
        private void SelectFaceShapeCategory()
        {
            if (selectedCategoryButton != null)
            {
                selectedCategoryButton.SetSelect(false);
            }
            
            faceCategoryButton.SetSelect(true);
            var button = categoryButtons.First(x => x.Category == Category.FaceShape);
            button.SetSelect(true);
            PanelSwitcher.Switch(Category.FaceShape);
            selectedCategoryButton = button;
        }
        
        private void SelectOutfitTopCategory()
        {
            if (selectedCategoryButton != null)
            {
                selectedCategoryButton.SetSelect(false);
            }

            outfitCategoryButton.SetSelect(true);
            var button = categoryButtons.First(x => x.Category == Category.Top);
            button.SetSelect(true);
            PanelSwitcher.Switch(Category.Top);
            selectedCategoryButton = button;
            OnCategorySelected?.Invoke(Category.Top);
        }

        private void DefaultZoom()
        {
            if (bodyType == BodyType.HalfBody)
            {
                cameraZoom.ToHalfBody();
            }
            else
            {
                cameraZoom.ToFullbodyView();
            }
        }

        private void SwitchZoomByCategory(Category category)
        {
            if (bodyType != BodyType.HalfBody)
            {
                if (category == Category.Outfit)
                {
                    cameraZoom.ToFullbodyView();
                }
                else
                {
                    cameraZoom.ToFaceView();
                }
            }
        }
    }
}
