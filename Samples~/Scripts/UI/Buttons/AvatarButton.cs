using System;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RawImage image;
        [SerializeField] private GameObject loading;
        [SerializeField] private GameObject buttonsPanel;
        [SerializeField] private Button customizeButton;
        [SerializeField] private Button selectButton;

        private string avatarId;
        private bool showButtons;

        private async void Start()
        {
            while (string.IsNullOrEmpty(avatarId))
            {
                await Task.Yield();
            }
            LoadImage();
        }

        public void Init(string id, Action onCustomize, Action onSelect, bool isCurrentPartner)
        {
            avatarId = id;
            customizeButton.onClick.AddListener(() => onCustomize());
            selectButton.onClick.AddListener(() => onSelect());
            showButtons = isCurrentPartner;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (showButtons)
            {
                buttonsPanel.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (showButtons)
            {
                buttonsPanel.SetActive(false);
            }
        }

        private async void LoadImage()
        {
            loading.SetActive(true);
            image.texture = await AvatarRenderHelper.GetPortrait(avatarId);
            loading.SetActive(false);
        }
    }
}
