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
            StateMachine.SetState(StateType.GenderSelection);
        }

        private void OnHalfBodySelected()
        {
            DataStore.AvatarProperties.BodyType = BodyType.HalfBody;
            StateMachine.SetState(StateType.GenderSelection);
        }
    }
}
