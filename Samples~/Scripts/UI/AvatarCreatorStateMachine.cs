using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorStateMachine : StateMachine
    {
        [SerializeField] private List<State> states;
        [SerializeField] private Button backButton;
        [SerializeField] private LoadingManager loadingManager;
        [SerializeField] private StateType startingState;
        [SerializeField] public AvatarCreatorData avatarCreatorData;
        [SerializeField] private ProfileManager profileManager;

        public Action<string> AvatarSaved;

        private void Start()
        {
            avatarCreatorData.AvatarProperties.Partner = CoreSettingsHandler.CoreSettings.Subdomain;
            Initialize();
            
            SetState(profileManager.LoadSession() ? StateType.AvatarSelection : startingState);
        }

        private void OnEnable()
        {
            StateChanged += OnStateChanged;
            AuthManager.OnSignedIn += OnSignedIn;
            AuthManager.OnSignedOut += OnSignedOut;
            AuthManager.OnSessionRefreshed += OnSessionRefreshed;
            backButton.onClick.AddListener(Back);
        }

        private void OnDisable()
        {
            StateChanged -= OnStateChanged;
            AuthManager.OnSignedIn -= OnSignedIn;
            AuthManager.OnSignedOut -= OnSignedOut;
            AuthManager.OnSessionRefreshed -= OnSessionRefreshed;
            backButton.onClick.RemoveListener(Back);
        }
        
        private void OnSignedIn(UserSession userSession)
        {
            profileManager.SaveSession(userSession);
        }

        private void OnSignedOut()
        {
            avatarCreatorData.AvatarProperties.Id = string.Empty;
            SetState(StateType.LoginWithCodeFromEmail);
            ClearPreviousStates();
        }
        
        private void OnSessionRefreshed(UserSession userSession)
        {
            profileManager.SaveSession(userSession);
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
            backButton.gameObject.SetActive(!CanShowBackButton(current));

            if (current == StateType.End)
            {
                AvatarSaved?.Invoke(avatarCreatorData.AvatarProperties.Id);
            }
        }

        private bool CanShowBackButton(StateType current)
        {
            return current == StateType.BodyTypeSelection || current == StateType.LoginWithCodeFromEmail || current == StateType.AvatarSelection;
        } 
    }
}
