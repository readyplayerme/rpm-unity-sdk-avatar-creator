using System.IO;
using System.Text;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class ProfileManager : MonoBehaviour
    {
        private const string FILE_PATH = "/Ready Player Me/user";

        [SerializeField] private ProfileUI profileUI;
        [SerializeField] private StateMachine stateMachine;

        private string path;

        private void Start()
        {
            path = Application.persistentDataPath + FILE_PATH;
            LoadSession();
        }

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

            SaveSession(userSession);
        }

        private void OnSignOut()
        {
            stateMachine.SetState(StateType.LoginWithCodeFromEmail);
            stateMachine.ClearPreviousStates();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void LoadSession()
        {
            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);
                var json = Encoding.UTF8.GetString(bytes);
                var userSession = JsonConvert.DeserializeObject<UserSession>(json);
                AuthManager.SetUser(userSession);
            }
        }

        private void SaveSession(UserSession userSession)
        {
            var json = JsonConvert.SerializeObject(userSession);
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
        }
    }
}
