using NativeAvatarCreator;
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

        private async void Login(string partnerDomain)
        {
            var startTime = Time.time;
            dataStore.AvatarProperties.Partner = partnerDomain;
            dataStore.User = await Auth.LoginAsAnonymous(partnerDomain);

            DebugPanel.AddLogWithDuration($"Logged in with userId: {dataStore.User.Id}", Time.time - startTime);
            authSelection.SetSelected();
        }
    }

}
