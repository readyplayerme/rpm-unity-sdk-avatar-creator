using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class GenderSelection : State
    {
        [SerializeField] private Button male;
        [SerializeField] private Button female;
        [SerializeField] private Button dontSpecify;

        public override StateType StateType => StateType.GenderSelection;

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
            NextState(OutfitGender.Masculine);
        }

        private void OnFemaleSelected()
        {
            NextState(OutfitGender.Feminine);
        }

        private void OnGenderNotSpecifiedSelected()
        {
            NextState(OutfitGender.None);
        }

        private void NextState(OutfitGender gender)
        {
            DataStore.AvatarProperties.Gender = gender;
            StateMachine.SetState(StateType.Editor);
        }
    }
}
