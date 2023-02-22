using UnityEngine;

namespace ReadyPlayerMe
{
    public enum StateType
    {
        None,
        Login,
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
        protected DataStore DataStore;
        protected GameObject Loading;

        public abstract StateType StateType { get; }

        public void Initialize(StateMachine stateMachine, DataStore dataStore, GameObject loading)
        {
            StateMachine = stateMachine;
            DataStore = dataStore;
            Loading = loading;
            gameObject.SetActive(false);
        }
    }
}
