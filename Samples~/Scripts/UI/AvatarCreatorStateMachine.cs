using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorStateMachine : StateMachine
    {
        [SerializeField] private List<State> states;
        [SerializeField] private Button button;
        [SerializeField] private LoadingManager loadingManager;
        [SerializeField] private StateType startingState;
        [SerializeField] public AvatarCreatorData avatarCreatorData;

        public Action<string> AvatarSaved;

        private void Start()
        {
            Initialize();
            SetState(startingState);
        }

        private void OnEnable()
        {
            StateChanged += OnStateChanged;
            button.onClick.AddListener(Back);
        }

        private void OnDisable()
        {
            StateChanged -= OnStateChanged;
            button.onClick.RemoveListener(Back);
        }

        private void Initialize()
        {
            foreach (var state in states)
            {
                state.Initialize(this, avatarCreatorData, loadingManager);
            }
            base.Initialize(states);
        }

        private void OnStateChanged(StateType current, StateType previous)
        {
            if (current == StateType.BodyTypeSelection || current == StateType.LoginWithCodeFromEmail)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
            }

            if (current == StateType.End)
            {
                AvatarSaved?.Invoke(avatarCreatorData.AvatarProperties.Id);
            }
        }
    }
}
