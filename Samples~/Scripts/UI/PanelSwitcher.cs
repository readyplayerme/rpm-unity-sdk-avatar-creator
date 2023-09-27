using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class PanelSwitcher
    {
        public static Dictionary<Category, GameObject> CategoryPanelMap { get; private set; }
        public static GameObject OutfitCategoryPanel;
        public static GameObject FaceCategoryPanel;

        public static void AddPanel(Category category, GameObject widget)
        {
            CategoryPanelMap ??= new Dictionary<Category, GameObject>();
            CategoryPanelMap.Add(category, widget);
        }

        public static void Switch(Category category)
        {
            DisableAllPanels();

            switch (category)
            {
                case Category.FaceShape:
                    FaceCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.SkinColor, true);
                    break;
                case Category.EyebrowStyle:
                    FaceCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.EyebrowColor, true);
                    break;
                case Category.BeardStyle:
                    FaceCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.BeardColor, true);
                    break;
                case Category.HairStyle:
                    SetActivePanel(category, true);
                    SetActivePanel(Category.HairColor, true);
                    break;
                case Category.NoseShape:
                case Category.LipShape:
                    FaceCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    break;
                case Category.EyeShape:
                    FaceCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.EyeColor, true);
                    break;
                case Category.Top:
                case Category.Bottom:
                case Category.Footwear:
                case Category.Outfit:
                    OutfitCategoryPanel.SetActive(true);
                    SetActivePanel(category, true);
                    break;
                default:
                    SetActivePanel(category, true);
                    break;
            }
        }

        private static void DisableAllPanels()
        {
            foreach (var panels in CategoryPanelMap)
            {
                panels.Value.SetActive(false);
            }
            
            FaceCategoryPanel.SetActive(false);
            OutfitCategoryPanel.SetActive(false);
        }

        private static void SetActivePanel(Category category, bool enable)
        {
            if (CategoryPanelMap.TryGetValue(category, out GameObject panel))
            {
                panel.SetActive(enable);
            }
        }
    }
}
