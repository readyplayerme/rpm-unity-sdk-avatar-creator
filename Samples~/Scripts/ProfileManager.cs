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

        private string path;

        private void Awake()
        {
            path = Application.persistentDataPath + FILE_PATH;
        }

        private void OnEnable()
        {
            profileUI.SignedOut += OnSignOut;
        }

        private void OnDisable()
        {
            profileUI.SignedOut -= OnSignOut;
        }

        public bool LoadSession()
        {
            if (!File.Exists(path))
            {
                return false;
            }
           
            var bytes = File.ReadAllBytes(path);
            var json = Encoding.UTF8.GetString(bytes);
            var userSession = JsonConvert.DeserializeObject<UserSession>(json);
            AuthManager.SetUser(userSession);
            SetProfileData(userSession);
            return true;

        }

        public void SaveSession(UserSession userSession)
        {
            var json = JsonConvert.SerializeObject(userSession);
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
            SetProfileData(userSession);
        }


        private void SetProfileData(UserSession userSession)
        {
            profileUI.SetProfileData(
                userSession.Name,
                char.ToUpperInvariant(userSession.Name[0]).ToString()
            );
        }

        private void OnSignOut()
        {
            AuthManager.Logout();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
