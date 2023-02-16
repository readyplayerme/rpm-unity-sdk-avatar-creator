using System;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AuthSelection : SelectionScreenBase
    {
        [SerializeField] private Button login;

        public Action Login;

        private void OnEnable()
        {
            login.onClick.AddListener(LoginAsAnonymous);
        }

        private void OnDisable()
        {
            login.onClick.RemoveListener(LoginAsAnonymous);
        }

        public void SetSelected()
        {
            IsSelected = true;
            Loading.SetActive(false);
        }

        private void LoginAsAnonymous()
        {
            Loading.SetActive(true);
            Login?.Invoke();
        }
    }
}
