using System;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AuthSelection : State
    {
        [SerializeField] private Button anonymousLoginButton;
        [SerializeField] private Button loginButton;
        public override StateType StateType => StateType.Login;
        public override StateType NextState => StateType.BodyTypeSelection;

        private void Start()
        {
            DataStore.AvatarProperties.Partner = CoreSettingsHandler.CoreSettings.Subdomain;
        }

        private void OnEnable()
        {
            anonymousLoginButton.onClick.AddListener(LoginAsAnonymous);
            loginButton.onClick.AddListener(LoginWithEmail);
        }

        private void OnDisable()
        {
            anonymousLoginButton.onClick.RemoveListener(LoginAsAnonymous);
            loginButton.onClick.RemoveListener(LoginWithEmail);
        }

        private async void LoginAsAnonymous()
        {
            Loading.SetActive(true);
            await Login();
            Loading.SetActive(false);
            StateMachine.SetState(NextState);
        }

        private async Task Login()
        {
            var startTime = Time.time;
             await AuthManager.LoginAsAnonymous();
            DebugPanel.AddLogWithDuration($"Logged in with userId: {AuthManager.UserSession.Id}", Time.time - startTime);
        }

        private void LoginWithEmail()
        {
            StateMachine.SetState(StateType.LoginWithCodeFromEmail);
        }
    }
}
