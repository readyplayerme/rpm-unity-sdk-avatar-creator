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
        [SerializeField] private DataStore dataStore;
        [SerializeField] private Button button;
        [SerializeField] private GameObject loading;
        [SerializeField] private OutfitGender defaultGender;
        [SerializeField] private BodyType defaultBodyType;

        public Action<string> AvatarSaved;

        private void Start()
        {
            Initialize(states);
            SetState(StateType.Login);
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

        protected override void Initialize(List<State> states)
        {
            dataStore.AvatarProperties.BodyType = defaultBodyType;
            dataStore.AvatarProperties.Gender = defaultGender;

            foreach (var state in states)
            {
                state.Initialize(this, dataStore, loading);
            }
            base.Initialize(states);
        }

        private void OnStateChanged(StateType current, StateType previous)
        {
            button.gameObject.SetActive(current != StateType.Login);

            if (current == StateType.End)
            {
                gameObject.SetActive(false);
                AvatarSaved?.Invoke(dataStore.AvatarId);
            }
        }
    }
}
