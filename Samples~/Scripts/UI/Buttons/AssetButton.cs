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
        
        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetColor(string colorHex)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color color);
            mask.GetComponent<Image>().color = color;
            icon.color = color;
            mask.GetComponent<Mask>().enabled = true;
            icon.rectTransform.localPosition = Vector3.zero;
            icon.rectTransform.sizeDelta = new Vector2(210, 210);
            
        }

        public void SetEyeColorConfig()
        {
            mask.GetComponent<Image>().color = Color.white;
            mask.GetComponent<Mask>().enabled = true;
            icon.rectTransform.localPosition = Vector3.zero;
            icon.rectTransform.sizeDelta = new Vector2(210, 210);
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
    }
}
