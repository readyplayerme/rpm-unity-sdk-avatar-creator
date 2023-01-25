using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarCreatorExample
{
    public class GenderSelection : SelectionScreenBase
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
            DataStore.AvatarProperties.Gender = OutfitGender.Masculine;
            IsSelected = true;
        }

        private void OnFemaleSelected()
        {
            DataStore.AvatarProperties.Gender = OutfitGender.Feminine;
            IsSelected = true;
        }

        private void OnGenderNotSpecifiedSelected()
        {
            DataStore.AvatarProperties.Gender = OutfitGender.None;
            IsSelected = true;
        }
    }
}
