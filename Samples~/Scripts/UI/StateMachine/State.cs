using UnityEngine;

namespace ReadyPlayerMe
{
    public enum StateType
    {
        None,
        Login,
        LoginWithCodeFromEmail,
        BodyTypeSelection,
        GenderSelection,
        SelfieSelection,
        CameraPhoto,
        Editor,
        End
    }

    public abstract class State : MonoBehaviour
    {
        protected StateMachine StateMachine;
        protected AvatarCreatorData AvatarCreatorData;
        protected GameObject Loading;

        public abstract StateType StateType { get; }
        public abstract StateType NextState { get; }

        public void Initialize(StateMachine stateMachine, AvatarCreatorData avatarCreatorData, GameObject loading)
        {
            StateMachine = stateMachine;
            AvatarCreatorData = avatarCreatorData;
            Loading = loading;
            gameObject.SetActive(false);
        }
    }
}
