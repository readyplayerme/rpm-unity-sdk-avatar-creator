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

        private async void Start()
        {
            while (string.IsNullOrEmpty(avatarId))
            {
                await Task.Yield();
            }
            LoadImage();
        }

        public void Init(string id, Action onCustomize, Action onSelect)
        {
            avatarId = id;
            customizeButton.onClick.AddListener(() => onCustomize());
            selectButton.onClick.AddListener(() => onSelect());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            buttonsPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            buttonsPanel.SetActive(false);
        }

        private async void LoadImage()
        {
            loading.SetActive(true);
            image.texture = await AvatarRenderHelper.GetPortrait(avatarId);
            loading.SetActive(false);
        }
    }
}
