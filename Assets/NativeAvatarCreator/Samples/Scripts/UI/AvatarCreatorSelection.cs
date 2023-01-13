using System;
using System.Collections.Generic;
using System.Linq;
using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AvatarCreatorSelection : SelectionPanel
    {
        [SerializeField] private GameObject assetButtonPrefab;
        [SerializeField] private GameObject assetTypePrefab;
        [SerializeField] private Transform assetTypeParent;
        [SerializeField] private GameObject assetTypePanelPrefab;
        [SerializeField] private Transform assetTypePanelParent;

        public Action Show;

        private Dictionary<string, Transform> assetTypes;

        private Transform selectedAssetType;

        public void OnEnable()
        {
            Loading.SetActive(true);
            Show?.Invoke();
        }


        public void InstantNoodles(Dictionary<PartnerAsset, Texture> assets)
        {
            assetTypes = new Dictionary<string, Transform>();

            foreach (var asset in assets)
            {
                Transform parent;
                if (assetTypes.ContainsKey(asset.Key.AssetType))
                {
                    parent = assetTypes[asset.Key.AssetType];
                }
                else
                {
                    var assetTypeButton = Instantiate(assetTypePrefab, assetTypeParent);
                    assetTypeButton.GetComponentInChildren<Text>().text = asset.Key.AssetType;

                    var assetTypePanel = Instantiate(assetTypePanelPrefab, assetTypePanelParent);
                    assetTypes.Add(asset.Key.AssetType, assetTypePanel.transform);
                    assetTypePanel.SetActive(false);
                    parent = assetTypePanel.transform;

                    assetTypeButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        selectedAssetType.gameObject.SetActive(false);
                        assetTypePanel.SetActive(true);
                        selectedAssetType = assetTypePanel.transform;
                    });
                }

                var assetButton = Instantiate(assetButtonPrefab, parent.GetChild(0).GetChild(0));
                assetButton.GetComponent<RawImage>().texture = asset.Value;
            }

            selectedAssetType = assetTypes.First().Value;
            selectedAssetType.gameObject.SetActive(true);

            Loading.SetActive(false);
        }

    }
}
