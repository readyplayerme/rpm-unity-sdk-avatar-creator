using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public static class PanelSwitcher
    {
        public static Dictionary<Category, GameObject> CategoryPanelMap { get; private set; }

        public static GameObject FaceTypePanel;

        private static Category currentCategory;

        public static void AddPanel(Category category, GameObject widget)
        {
            CategoryPanelMap ??= new Dictionary<Category, GameObject>();
            CategoryPanelMap.Add(category, widget);
        }

        public static void Clear()
        {
            if (CategoryPanelMap == null)
            {
                return;
            }
            foreach (var assetTypePanels in CategoryPanelMap)
            {
                foreach (Transform child in assetTypePanels.Value.GetComponent<ScrollRect>().content)
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }

        public static void Switch(Category category)
        {
            SetActivePanel(currentCategory, false);
            DisableColorPanels();

            switch (category)
            {
                case Category.FaceShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.SkinColor, true);
                    break;
                case Category.EyebrowStyle:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.EyebrowColor, true);
                    break;
                case Category.BeardStyle:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.BeardColor, true);
                    break;
                case Category.HairStyle:
                    FaceTypePanel.SetActive(false);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.HairColor, true);
                    break;
                case Category.NoseShape:
                case Category.LipShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(category, true);
                    break;
                case Category.EyeShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(category, true);
                    SetActivePanel(Category.EyeColor, true);
                    break;
                default:
                    FaceTypePanel.SetActive(false);
                    SetActivePanel(category, true);
                    break;
            }

            currentCategory = category;
        }
        
        private static void DisableColorPanels()
        {
            SetActivePanel(Category.EyeColor, false);
            SetActivePanel(Category.SkinColor, false);
            SetActivePanel(Category.BeardColor, false);
            SetActivePanel(Category.HairColor, false);
            SetActivePanel(Category.EyebrowColor, false);
        }

        private static void SetActivePanel(Category category, bool enable)
        {
            if (CategoryPanelMap.ContainsKey(category))
            {
                CategoryPanelMap[category].SetActive(enable);
            }
        }
    }
}
