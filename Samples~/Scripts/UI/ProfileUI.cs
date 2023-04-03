using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class ProfileUI : MonoBehaviour
    {
        [SerializeField] private Text username;
        [SerializeField] private GameObject userPanel;
        [SerializeField] private Button profileButton;
        [SerializeField] private Text profileText;
        [SerializeField] private Button signOutButton;

        public Action SignedOut;

        private void OnEnable()
        {
            profileButton.onClick.AddListener(ToggleProfilePanel);
            signOutButton.onClick.AddListener(OnSignOutButton);
        }

        private void OnDisable()
        {
            profileButton.onClick.RemoveListener(ToggleProfilePanel);
            signOutButton.onClick.RemoveListener(OnSignOutButton);
        }

        public void SetProfileData(string user, string profileButtonText)
        {
            username.text = user;
            profileText.text = profileButtonText;
            profileButton.gameObject.SetActive(true);
        }

        private void OnSignOutButton()
        {
            profileButton.gameObject.SetActive(false);
            ToggleProfilePanel();
            SignedOut?.Invoke();
        }

        private void ToggleProfilePanel()
        {
            userPanel.SetActive(!userPanel.activeSelf);
        }
    }
}
