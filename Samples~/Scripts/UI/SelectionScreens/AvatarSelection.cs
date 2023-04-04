using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarSelection : State
    {
        [SerializeField] private Button partnerAvatarsButton;
        [SerializeField] private Button allAvatarsButton;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform parent;
        public override StateType StateType => StateType.AvatarSelection;
        public override StateType NextState => StateType.Editor;

        private Dictionary<string, string> avatarPartnerMap;
        private Dictionary<string, GameObject> avatarButtonsMap;
        private AvatarAPIRequests avatarAPIRequests;

        private async void CreateAvatarButtons()
        {
            if (avatarPartnerMap != null && avatarPartnerMap.Count != 0)
            {
                return;
            }

            LoadingManager.EnableLoading();

            avatarAPIRequests = new AvatarAPIRequests();
            var avatarPartnerData = await avatarAPIRequests.FetchAvatar(AuthManager.UserSession.Id);

            avatarPartnerMap = new Dictionary<string, string>();
            avatarButtonsMap = new Dictionary<string, GameObject>();
          
            foreach (var data in avatarPartnerData)
            {
                avatarPartnerMap.Add(data.Id, data.Partner);
            }
            
            foreach (var avatar in avatarPartnerMap)
            {
                CreateButton(avatar.Key);
            }

            OnPartnerAvatarsButton();
            LoadingManager.DisableLoading();
        }

        private void OnEnable()
        {
            partnerAvatarsButton.onClick.AddListener(OnPartnerAvatarsButton);
            allAvatarsButton.onClick.AddListener(OnAllAvatarsButton);
            AuthManager.OnSignedOut += OnSignedOut;

            CreateAvatarButtons();
        }

        private void OnDisable()
        {
            partnerAvatarsButton.onClick.RemoveListener(OnPartnerAvatarsButton);
            allAvatarsButton.onClick.RemoveListener(OnAllAvatarsButton);
            AuthManager.OnSignedOut -= OnSignedOut;
        }

        private void OnSignedOut()
        {
            foreach (var avatars in avatarButtonsMap)
            {
                Destroy(avatars.Value);
            }
            avatarButtonsMap.Clear();
            avatarPartnerMap.Clear();
        }

        private void OnAllAvatarsButton()
        {
            foreach (var avatar in avatarPartnerMap)
            {
                avatarButtonsMap[avatar.Key].SetActive(true);
            }
        }

        private void OnPartnerAvatarsButton()
        {
            foreach (var avatar in avatarPartnerMap)
            {
                avatarButtonsMap[avatar.Key].SetActive(avatar.Value == AvatarCreatorData.AvatarProperties.Partner);
            }
        }

        private void CreateButton(string avatarId)
        {
            var button = Instantiate(buttonPrefab, parent);
            button.GetComponent<AvatarButton>().Init(avatarId, () => OnCustomize(avatarId),()=> OnSelected(avatarId));
            avatarButtonsMap.Add(avatarId, button);
        }


        private async void OnCustomize(string avatarId)
        {
            AvatarCreatorData.AvatarProperties.Id = avatarId;
            AvatarCreatorData.AvatarProperties = await avatarAPIRequests.GetAvatarMetadata(avatarId);
            StateMachine.SetState(StateType.Editor);
        }
        
        private void OnSelected(string avatarId)
        {
            AvatarCreatorData.AvatarProperties.Id = avatarId;
            StateMachine.SetState(StateType.End);
        }
    }
}
