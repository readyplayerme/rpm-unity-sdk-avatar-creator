using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class PanelSwitcher
    {
        public static Dictionary<AssetType, GameObject> AssetTypePanelMap { get; private set; }

        public static GameObject FaceTypePanel;

        private static AssetType currentAssetType;

        public static void AddPanel(AssetType assetType, GameObject widget)
        {
            AssetTypePanelMap ??= new Dictionary<AssetType, GameObject>();
            AssetTypePanelMap.Add(assetType, widget);
        }

        public static void Clear()
        {
            if (AssetTypePanelMap == null)
            {
                return;
            }
            foreach (var assetTypePanels in AssetTypePanelMap)
            {
                Object.Destroy(assetTypePanels.Value);
            }
            AssetTypePanelMap.Clear();
        }

        public static void Switch(AssetType assetType)
        {
            SetActivePanel(currentAssetType, false);
            DisableColorPanels();

            switch (assetType)
            {
                case AssetType.FaceShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(assetType, true);
                    SetActivePanel(AssetType.SkinColor, true);
                    break;
                case AssetType.EyebrowStyle:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(assetType, true);
                    SetActivePanel(AssetType.EyebrowColor, true);
                    break;
                case AssetType.BeardStyle:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(assetType, true);
                    SetActivePanel(AssetType.BeardColor, true);
                    break;
                case AssetType.HairStyle:
                    FaceTypePanel.SetActive(false);
                    SetActivePanel(assetType, true);
                    SetActivePanel(AssetType.HairColor, true);
                    break;
                case AssetType.NoseShape:
                case AssetType.LipShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(assetType, true);
                    break;
                case AssetType.EyeShape:
                    FaceTypePanel.SetActive(true);
                    SetActivePanel(assetType, true);
                    SetActivePanel(AssetType.EyeColor, true);
                    break;
                default:
                    FaceTypePanel.SetActive(false);
                    SetActivePanel(assetType, true);
                    break;
            }

            currentAssetType = assetType;
        }
        
        private static void DisableColorPanels()
        {
            SetActivePanel(AssetType.EyeColor, false);
            SetActivePanel(AssetType.SkinColor, false);
            SetActivePanel(AssetType.BeardColor, false);
            SetActivePanel(AssetType.HairColor, false);
            SetActivePanel(AssetType.EyebrowColor, false);
        }

        private static void SetActivePanel(AssetType assetType, bool enable)
        {
            if (AssetTypePanelMap.ContainsKey(assetType))
            {
                AssetTypePanelMap[assetType].SetActive(enable);
            }
        }
    }
}
