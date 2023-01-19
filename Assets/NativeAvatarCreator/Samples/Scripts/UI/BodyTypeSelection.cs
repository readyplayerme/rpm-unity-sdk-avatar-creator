using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class BodyTypeSelection : SelectionPanel
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
            DataStore.AvatarProperties.BodyType = AvatarPropertiesConstants.FULL_BODY;
            IsSelected = true;
        }

        private void OnHalfBodySelected()
        {
            DataStore.AvatarProperties.BodyType = AvatarPropertiesConstants.HALF_BODY;
            IsSelected = true;
        }
    }
}
