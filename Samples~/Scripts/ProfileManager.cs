using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class ProfileManager : MonoBehaviour
    {
        [SerializeField] private ProfileUI profileUI;
        [SerializeField] private StateMachine stateMachine;

        private void OnEnable()
        {
            AuthManager.SignedIn += OnSignIn;
            profileUI.SignedOut += OnSignOut;
        }

        private void OnDisable()
        {
            AuthManager.SignedIn -= OnSignIn;
            profileUI.SignedOut -= OnSignOut;
        }

        private void OnSignIn(UserSession userSession)
        {
            profileUI.SetProfileData(
                userSession.Name,
                char.ToUpperInvariant(userSession.Name[0]).ToString()
            );
        }

        private void OnSignOut()
        {
            stateMachine.SetState(StateType.LoginWithCodeFromEmail);
            stateMachine.ClearPreviousStates();
        }
    }
}
