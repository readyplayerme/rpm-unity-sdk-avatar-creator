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

        private string partnerDomain;

        private void Start()
        {
            partnerDomain = CoreSettingsHandler.CoreSettings.Subdomain;
            DataStore.AvatarProperties.Partner = partnerDomain;
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
            var authManager = new AuthManager(partnerDomain);
            DataStore.User = await authManager.LoginAsAnonymous();
            DebugPanel.AddLogWithDuration($"Logged in with userId: {DataStore.User.Id}", Time.time - startTime);
        }

        private void LoginWithEmail()
        {
            StateMachine.SetState(StateType.LoginWithCodeFromEmail);
        }
    }
}
