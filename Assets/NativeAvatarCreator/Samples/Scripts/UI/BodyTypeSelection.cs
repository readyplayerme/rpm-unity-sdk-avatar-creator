using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class BodyTypeSelection : SelectionScreen
    {
        [SerializeField] private Button fullBody;
        [SerializeField] private Button halfBody;

        private void OnEnable()
        {
            fullBody.onClick.AddListener(OnFullBodySelected);
            halfBody.onClick.AddListener(OnHalfBodySelected);
        }

        private void OnDisable()
        {
            fullBody.onClick.RemoveListener(OnFullBodySelected);
            halfBody.onClick.RemoveListener(OnHalfBodySelected);
        }

        private void OnFullBodySelected()
        {
            DataStore.AvatarProperties.BodyType = BodyType.FullBody;
            IsSelected = true;
        }

        private void OnHalfBodySelected()
        {
            DataStore.AvatarProperties.BodyType = BodyType.HalfBody;
            IsSelected = true;
        }
    }
}
