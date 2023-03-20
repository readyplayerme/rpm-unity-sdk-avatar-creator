using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AssetButton : MonoBehaviour
    {
        [SerializeField] private RawImage icon;
        [SerializeField] private GameObject selected;
        [SerializeField] private Button button;
        [SerializeField] private GameObject mask;
        [SerializeField] private GameObject loading;
        
        private readonly Vector2 DefaultEyeSize = new Vector2(210, 210);
        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetColor(string colorHex)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color color);
            icon.color = color;
        }

        public void SetEyeColorConfig()
        {
            EnableMask();
            icon.rectTransform.localPosition = Vector3.zero;
            icon.rectTransform.sizeDelta = DefaultEyeSize;
        }

        public void SetIcon(Texture texture)
        {
            icon.texture = texture;
            icon.enabled = true;
            loading.SetActive(false);
        }

        public void SetSelect(bool isSelected)
        {
            selected.SetActive(isSelected);
        }

        private void EnableMask()
        {
            mask.GetComponent<Mask>().enabled = true;
            mask.GetComponent<Image>().color = Color.white;
        }
    }
}
