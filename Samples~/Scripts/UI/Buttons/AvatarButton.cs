using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarButton : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private GameObject loading;

        [HideInInspector] public string avatarId;

        private async void Start()
        {
            while (string.IsNullOrEmpty(avatarId))
            {
                await Task.Yield();
            }
            LoadImage();
        }

        private async void LoadImage()
        {
            loading.SetActive(true);
            image.texture = await AvatarRenderHelper.GetPortrait(avatarId);
            loading.SetActive(false);
        }
    }
}
