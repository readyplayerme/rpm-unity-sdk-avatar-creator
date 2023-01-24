using NativeAvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class GenderSelection : SelectionScreen
    {
        [SerializeField] private Button male;
        [SerializeField] private Button female;
        [SerializeField] private Button dontSpecify;

        private void OnEnable()
        {
            male.onClick.AddListener(OnMaleSelected);
            female.onClick.AddListener(OnFemaleSelected);
            dontSpecify.onClick.AddListener(OnGenderNotSpecifiedSelected);
        }

        private void OnDisable()
        {
            male.onClick.RemoveListener(OnMaleSelected);
            female.onClick.RemoveListener(OnFemaleSelected);
            dontSpecify.onClick.RemoveListener(OnGenderNotSpecifiedSelected);
        }

        private void OnMaleSelected()
        {
            DataStore.AvatarProperties.Gender = AvatarPropertiesConstants.MALE;
            IsSelected = true;
        }

        private void OnFemaleSelected()
        {
            DataStore.AvatarProperties.Gender = AvatarPropertiesConstants.FEMALE;
            IsSelected = true;
        }

        private void OnGenderNotSpecifiedSelected()
        {
            DataStore.AvatarProperties.Gender = "";
            IsSelected = true;
        }
    }
}
