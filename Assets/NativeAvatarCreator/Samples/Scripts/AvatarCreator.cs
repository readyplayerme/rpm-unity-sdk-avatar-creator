using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AvatarCreator : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private AvatarLoader avatarLoader;
        [SerializeField] private AvatarConfig avatarConfig;

        private AvatarAPIRequests avatarAPIRequests;

        private string avatarId;
        private GameObject avatar;

        private string avatarConfigParameters;

        private void Start()
        {
            avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
        }

        private void OnEnable()
        {
            avatarCreatorSelection.Show += Show;
            avatarCreatorSelection.Hide += Hide;
        }

        private void OnDisable()
        {
            avatarCreatorSelection.Show -= Show;
            avatarCreatorSelection.Hide -= Hide;
        }

        private async void Show()
        {
            await CreateDefaultModel();
        }

        private void Hide()
        {
            Destroy(avatar);
        }

        private async Task CreateDefaultModel()
        {
            var startTime = Time.time;

            avatarAPIRequests = new AvatarAPIRequests(dataStore.User.Token);

            dataStore.AvatarProperties.Assets = AvatarPropertiesConstants.DefaultAssets;
            avatarId = await avatarAPIRequests.Create(dataStore.AvatarProperties);

            var timeForCreateRequest = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar metadata created in temp storage", timeForCreateRequest);
            startTime = timeForCreateRequest;

            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId, avatarConfigParameters);
            var timeForGettingPreviewAvatar = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Downloaded preview avatar", timeForGettingPreviewAvatar);
            startTime = timeForGettingPreviewAvatar;

            avatar = await avatarLoader.LoadAvatar(avatarId, data);
            var avatarLoadingTime = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar loaded", avatarLoadingTime);
        }

        public async void UpdateAvatar(string assetId, AssetType assetType)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);

            var data = await avatarAPIRequests.UpdateAvatar(avatarId, payload, avatarConfigParameters);
            avatar = await avatarLoader.LoadAvatar(avatarId, data);
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        public async void Save()
        {
            await avatarAPIRequests.SaveAvatar(avatarId);
        }
    }
}
