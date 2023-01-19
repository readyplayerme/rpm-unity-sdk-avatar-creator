using System;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AssetTypeButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Color selectedColor;

        private Color defaultColor;

        private void Awake()
        {
            defaultColor = icon.color;
        }

        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetSelect(bool isSelected)
        {
            icon.color = isSelected ? selectedColor : defaultColor;
        }
    }
}
