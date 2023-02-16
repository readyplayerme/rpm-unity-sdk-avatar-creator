using System.Collections.Generic;
using System.Linq;
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
            foreach (var assetTypePanels in AssetTypePanelMap)
            {
                Object.Destroy(assetTypePanels.Value);
            }
            AssetTypePanelMap.Clear();
        }

        public static void Switch(AssetType assetType)
        {
            if (AssetTypePanelMap.Keys.Contains(currentAssetType))
            {
                SetActivePanel(currentAssetType, false);
            }

            SetActivePanel(AssetType.EyeColor, false);

            switch (assetType)
            {
                case AssetType.FaceShape:
                case AssetType.EyebrowStyle:
                case AssetType.NoseShape:
                case AssetType.LipShape:
                case AssetType.BeardStyle:
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

        private static void SetActivePanel(AssetType assetType, bool enable)
        {
            AssetTypePanelMap[assetType].SetActive(enable);
        }
    }
}
