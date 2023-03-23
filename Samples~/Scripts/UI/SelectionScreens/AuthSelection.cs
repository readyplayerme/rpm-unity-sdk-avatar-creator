using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace ReadyPlayerMe
{
    public class AuthSelection : State
    {
        [SerializeField] private Button login;
        public override StateType StateType => StateType.Login;
        public override StateType NextState => StateType.BodyTypeSelection;

        private void OnEnable()
        {
            login.onClick.AddListener(LoginAsAnonymous);
        }

        private void OnDisable()
        {
            login.onClick.RemoveListener(LoginAsAnonymous);
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
            var partnerDomain = CoreSettingsHandler.CoreSettings.Subdomain;
            DataStore.AvatarProperties.Partner = partnerDomain;

            var authManager = new AuthManager(partnerDomain);
            DataStore.User = await authManager.LoginAsAnonymous();

            DebugPanel.AddLogWithDuration($"Logged in with userId: {DataStore.User.Id}", Time.time - startTime);
        }
    }
}
