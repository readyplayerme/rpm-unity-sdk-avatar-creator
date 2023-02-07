using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AuthManager : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AuthSelection authSelection;

        public void OnEnable()
        {
            authSelection.Login += Login;
        }

        private async void Login()
        {
            var startTime = Time.time;
            var partnerDomain = CoreSettings.PartnerSubdomainSettings.Subdomain;
            dataStore.AvatarProperties.Partner = partnerDomain;

            dataStore.User = await AuthRequests.LoginAsAnonymous(partnerDomain);

            DebugPanel.AddLogWithDuration($"Logged in with userId: {dataStore.User.Id}", Time.time - startTime);
            authSelection.SetSelected();
        }
    }

}
