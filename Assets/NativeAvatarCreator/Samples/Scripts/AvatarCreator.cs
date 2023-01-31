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
        [SerializeField] private GameObject avatarCreatorUI;
        [SerializeField] private AvatarConfig inCreatorConfig;
        [SerializeField] private AvatarConfig inGameConfig;

        private AvatarAPIRequests avatarAPIRequests;

        private string avatarId;
        private GameObject avatar;

        private string avatarConfigParameters;

        private void Start()
        {
            avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(inCreatorConfig);
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

            dataStore.AvatarProperties.Assets = AvatarPropertiesConstants.DefaultAssets;
            avatarId = await avatarAPIRequests.Create(dataStore.AvatarProperties);

            var timeForCreateRequest = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar metadata created in temp storage", timeForCreateRequest);
            startTime = timeForCreateRequest;

            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId, avatarConfigParameters);
            var timeForGettingPreviewAvatar = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Downloaded preview avatar", timeForGettingPreviewAvatar);
            startTime = timeForGettingPreviewAvatar;

            avatar = await avatarLoader.LoadAvatar(avatarId, dataStore.AvatarProperties.BodyType, dataStore.AvatarProperties.Gender, data);
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
            avatar = await avatarLoader.LoadAvatar(avatarId, dataStore.AvatarProperties.BodyType, dataStore.AvatarProperties.Gender, data);
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        private async void Save()
        {
            avatarCreatorUI.gameObject.SetActive(false);
            var startTime = Time.time;
            await avatarAPIRequests.SaveAvatar(avatarId);
            DebugPanel.AddLogWithDuration("Avatar saved", Time.time - startTime);

            var avatarObjectLoader = new AvatarObjectLoader();
            avatarObjectLoader.AvatarConfig = inGameConfig;
            avatarObjectLoader.OnCompleted += (sender, args) =>
            {
                AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, args.Avatar);
                DebugPanel.AddLogWithDuration("Created avatar loaded", Time.time - startTime);
            };

            avatarObjectLoader.LoadAvatar($"{Endpoints.AVATAR_API_V1}/{avatarId}.glb");
        }
    }
}
