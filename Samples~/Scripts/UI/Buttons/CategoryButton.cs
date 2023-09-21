using System;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class CategoryButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Category category;

        public Category Category => category;

        private Action onClickAction;

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
