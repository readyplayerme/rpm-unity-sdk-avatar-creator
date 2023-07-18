using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class BodyTypeSelection : State
    {
        [SerializeField] private Button fullBody;
        [SerializeField] private Button halfBody;
        public override StateType StateType => StateType.BodyTypeSelection;
        public override StateType NextState => StateType.GenderSelection;

        public override void ActivateState()
        {
            fullBody.onClick.AddListener(OnFullBodySelected);
            halfBody.onClick.AddListener(OnHalfBodySelected);
        }

        public override void DeactivateState()
        {
            fullBody.onClick.RemoveListener(OnFullBodySelected);
            halfBody.onClick.RemoveListener(OnHalfBodySelected);
        }

        private void OnFullBodySelected()
        {
            AvatarCreatorData.AvatarProperties.BodyType = BodyType.FullBody;
            StateMachine.SetState(NextState);
        }

        private void OnHalfBodySelected()
        {
            AvatarCreatorData.AvatarProperties.BodyType = BodyType.HalfBody;
            StateMachine.SetState(NextState);
        }
    }
}
