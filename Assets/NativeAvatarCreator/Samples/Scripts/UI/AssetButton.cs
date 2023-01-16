using System;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class AssetButton : MonoBehaviour
    {
        [SerializeField] private RawImage icon;
        [SerializeField] private GameObject selected;
        [SerializeField] private Button button;

        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetIcon(Texture texture)
        {
            icon.texture = texture;
        }

        public void SetSelect(bool isSelected)
        {
            selected.SetActive(isSelected);
        }
    }
}
