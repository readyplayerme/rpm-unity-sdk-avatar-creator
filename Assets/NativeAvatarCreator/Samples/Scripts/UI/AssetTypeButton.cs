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
            icon.color = isSelected ? selectedColor : Color.white;
        }
    }
}
