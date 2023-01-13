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
            dataStore.Payload.Partner = partnerDomain;
            dataStore.User = await Auth.LoginAsAnonymous(partnerDomain);

            Debug.Log($"Logged in with userId: {dataStore.User.Id}");
            authSelection.SetSelected();
        }
    }

}
