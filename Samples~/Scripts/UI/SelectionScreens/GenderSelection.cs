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
        public override StateType NextState => StateType.SelfieSelection;


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
            SetNextState(OutfitGender.Masculine);
        }

        private void OnFemaleSelected()
        {
            SetNextState(OutfitGender.Feminine);
        }

        private void OnGenderNotSpecifiedSelected()
        {
            SetNextState(OutfitGender.None);
        }

        private void SetNextState(OutfitGender gender)
        {
            AvatarCreatorData.AvatarProperties.Gender = gender;
            StateMachine.SetState(NextState);
        }
    }
}
