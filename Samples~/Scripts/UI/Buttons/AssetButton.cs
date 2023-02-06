using System;
using AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AssetButton : MonoBehaviour
    {
        [SerializeField] private RawImage icon;
        [SerializeField] private GameObject selected;
        [SerializeField] private Button button;
        [SerializeField] private GameObject mask;

        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetIcon(AssetType assetType, Texture texture)
        {
            icon.texture = texture;
            icon.enabled = true;
            if (assetType == AssetType.EyeColor)
            {
                mask.GetComponent<Image>().color = Color.white;
                mask.GetComponent<Mask>().enabled = true;
                icon.rectTransform.localPosition = Vector3.zero;
                icon.rectTransform.sizeDelta = new Vector2(210, 210);
            }
        }

        public void SetSelect(bool isSelected)
        {
            selected.SetActive(isSelected);
        }
    }
}
