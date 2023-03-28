using ReadyPlayerMe.AvatarLoader;
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
