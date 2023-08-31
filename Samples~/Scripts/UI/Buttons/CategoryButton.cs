using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class CategoryButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Color selectedColor;

        private Color defaultColor;
        private Action onClickAction;

        private void Awake()
        {
            defaultColor = icon.color;
        }

        public void AddListener(Action action)
        {
            onClickAction = action;
            button.onClick.AddListener(action.Invoke);
        }

        public void RemoveListener()
        {
            button.onClick.RemoveListener(onClickAction.Invoke);
        }
        
        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetSelect(bool isSelected)
        {
            icon.color = isSelected ? selectedColor : defaultColor;
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }
    }
}
