using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarCreatorManager : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private GameObject avatarCreatorUI;
        [SerializeField] private AvatarConfig inCreatorConfig;
        [SerializeField] private RuntimeAnimatorController animator;

        public event Action<string> Saved;

        private AvatarAPIRequests avatarAPIRequests;
        private InCreatorAvatarLoader inCreatorAvatarLoader;
        private string avatarId;
        private GameObject avatar;

        private string avatarConfigParameters;

        private void Start()
        {
            avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(inCreatorConfig);
            inCreatorAvatarLoader = new InCreatorAvatarLoader();
        }

        private void OnEnable()
        {
            avatarCreatorSelection.Show += Show;
            avatarCreatorSelection.Hide += Hide;

            avatarCreatorSelection.OnClick += UpdateAvatar;
            avatarCreatorSelection.Save += Save;
        }

        private void OnDisable()
        {
            avatarCreatorSelection.Show -= Show;
            avatarCreatorSelection.Hide -= Hide;

            avatarCreatorSelection.OnClick -= UpdateAvatar;
            avatarCreatorSelection.Save -= Save;
        }

        private async void Show()
        {
            await CreateDefaultModel();
        }

        private void Hide()
        {
            if (avatar != null)
            {
                Destroy(avatar);
            }
        }

        private async Task CreateDefaultModel()
        {
            var startTime = Time.time;
            avatarAPIRequests = new AvatarAPIRequests(dataStore.User.Token);

            dataStore.AvatarProperties.Assets = dataStore.AvatarProperties.Gender == OutfitGender.Feminine ? AvatarPropertiesConstants.FemaleDefaultAssets : AvatarPropertiesConstants.MaleDefaultAssets;
            avatarId = await avatarAPIRequests.Create(dataStore.AvatarProperties);

            var timeForCreateRequest = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar metadata created in temp storage", timeForCreateRequest);
            startTime = timeForCreateRequest;

            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId, avatarConfigParameters);
            var timeForGettingPreviewAvatar = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Downloaded preview avatar", timeForGettingPreviewAvatar);
            startTime = timeForGettingPreviewAvatar;

            avatar = await inCreatorAvatarLoader.Load(avatarId, dataStore.AvatarProperties.BodyType, dataStore.AvatarProperties.Gender, data);
            ProcessAvatar();

            var avatarLoadingTime = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar loaded", avatarLoadingTime);
            avatarCreatorSelection.Loading.SetActive(false);
        }

        private async void UpdateAvatar(string assetId, AssetType assetType)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);

            var data = await avatarAPIRequests.UpdateAvatar(avatarId, payload, avatarConfigParameters);
            avatar = await inCreatorAvatarLoader.Load(avatarId, dataStore.AvatarProperties.BodyType, dataStore.AvatarProperties.Gender, data);
            ProcessAvatar();

            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        private async void Save()
        {
            avatarCreatorUI.gameObject.SetActive(false);
            var startTime = Time.time;
            await avatarAPIRequests.SaveAvatar(avatarId);
            DebugPanel.AddLogWithDuration("Avatar saved", Time.time - startTime);
            Saved?.Invoke(avatarId);
        }

        private void ProcessAvatar()
        {
            avatar.GetComponent<Animator>().runtimeAnimatorController = animator;
            avatar.AddComponent<RotateAvatar>();
            if (dataStore.AvatarProperties.BodyType == BodyType.FullBody)
            {
                avatar.GetComponent<Animator>().runtimeAnimatorController = animator;
            }
        }
    }
}
