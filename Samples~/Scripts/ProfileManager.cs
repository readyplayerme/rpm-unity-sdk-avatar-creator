using System.IO;
using System.Text;
using Newtonsoft.Json;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class ProfileManager : MonoBehaviour
    {
        private const string DIRECTORY_NAME = "Ready Player Me";
        private const string FILE_NAME = "User";

        [SerializeField] private ProfileUI profileUI;

        private string filePath;
        private string directoryPath;

        private void Awake()
        {
            directoryPath = $"{Application.persistentDataPath}/{DIRECTORY_NAME}";
            filePath = $"{directoryPath}/{FILE_NAME}";
        }

        private void OnEnable()
        {
            profileUI.SignedOut += AuthManager.Logout;
            AuthManager.OnSignedOut += DeleteSession;
        }


        private void OnDisable()
        {
            profileUI.SignedOut -= AuthManager.Logout;
            AuthManager.OnSignedOut -= DeleteSession;
        }

        public bool LoadSession()
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            var bytes = File.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            var userSession = JsonConvert.DeserializeObject<UserSession>(json);
            AuthManager.SetUser(userSession);
            SetProfileData(userSession);
            return true;

        }

        public void SaveSession(UserSession userSession)
        {
            var json = JsonConvert.SerializeObject(userSession);
            DirectoryUtility.ValidateDirectory(directoryPath);
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(json));
            SetProfileData(userSession);
        }


        private void SetProfileData(UserSession userSession)
        {
            profileUI.SetProfileData(
                userSession.Name,
                char.ToUpperInvariant(userSession.Name[0]).ToString()
            );
        }
        
        private void DeleteSession()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            profileUI.ClearProfile();
        }

    }
}
