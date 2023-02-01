using NativeAvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AuthManager : MonoBehaviour
    {
        [SerializeField] private AuthSelection authSelection;
        [SerializeField] private DataStore dataStore;

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
