using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorStateMachine : StateMachine
    {
        [SerializeField] private List<State> states;
        [SerializeField] private Button button;
        [SerializeField] private GameObject loading;
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
                state.Initialize(this, avatarCreatorData, loading);
            }
            base.Initialize(states);
        }

        private void OnStateChanged(StateType current, StateType previous)
        {
            button.gameObject.SetActive(current != StateType.Login);

            if (current == StateType.End)
            {
                AvatarSaved?.Invoke(avatarCreatorData.AvatarId);
            }
        }
    }
}
