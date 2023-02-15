using System;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
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
        [SerializeField] private AuthSelection authSelection;

        public event Action<string> Saved;

        private GameObject avatar;
        private AvatarManager avatarManager;

        private void OnEnable()
        {
            authSelection.Login += Login;

            avatarCreatorSelection.Show += Show;
            avatarCreatorSelection.Hide += Hide;

            avatarCreatorSelection.OnClick += UpdateAvatar;
            avatarCreatorSelection.Save += Save;
        }

        private void OnDisable()
        {
            authSelection.Login -= Login;

            avatarCreatorSelection.Show -= Show;
            avatarCreatorSelection.Hide -= Hide;

            avatarCreatorSelection.OnClick -= UpdateAvatar;
            avatarCreatorSelection.Save -= Save;
        }

        private async void Login()
        {
            var startTime = Time.time;
            var partnerDomain = CoreSettingsHandler.CoreSettings.Subdomain;
            dataStore.AvatarProperties.Partner = partnerDomain;

            var authManager = new AuthManager(partnerDomain);
            dataStore.User = await authManager.Login();

            DebugPanel.AddLogWithDuration($"Logged in with userId: {dataStore.User.Id}", Time.time - startTime);
            authSelection.SetSelected();
        }

        private void Show()
        {
            LoadAssets();
            CreateDefaultModel();
        }

        private void Hide()
        {
            if (avatar != null)
            {
                Destroy(avatar);
            }
        }

        private async void LoadAssets()
        {
            var startTime = Time.time;
            var assetIconDownloadTasks = await PartnerAssetsManager.GetAllAssets(
                dataStore.User.Token,
                dataStore.AvatarProperties.Partner,
                dataStore.AvatarProperties.BodyType,
                dataStore.AvatarProperties.Gender
            );

            DebugPanel.AddLogWithDuration("Got all partner assets", Time.time - startTime);
            avatarCreatorSelection.AddAllAssetButtons(dataStore.AvatarProperties.BodyType, assetIconDownloadTasks);
        }

        private async void CreateDefaultModel()
        {
            avatarManager = new AvatarManager(dataStore.User.Token, inCreatorConfig);

            var startTime = Time.time;
            dataStore.AvatarProperties.Assets = dataStore.AvatarProperties.Gender == OutfitGender.Feminine
                ? AvatarPropertiesConstants.FemaleDefaultAssets
                : AvatarPropertiesConstants.MaleDefaultAssets;
            avatar = await avatarManager.Create(dataStore.AvatarProperties);

            var avatarLoadingTime = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar loaded", avatarLoadingTime);

            ProcessAvatar();
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

            avatar = await avatarManager.Update(dataStore.AvatarProperties.BodyType, dataStore.AvatarProperties.Gender, assetId, assetType);
            ProcessAvatar();
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        private async void Save()
        {
            avatarCreatorUI.gameObject.SetActive(false);
            var startTime = Time.time;
            var avatarId = await avatarManager.Save();
            DebugPanel.AddLogWithDuration("Avatar saved", Time.time - startTime);
            Saved?.Invoke(avatarId);
        }

        private void ProcessAvatar()
        {
            if (dataStore.AvatarProperties.BodyType == BodyType.FullBody)
            {
                avatar.GetComponent<Animator>().runtimeAnimatorController = animator;
            }
            avatar.AddComponent<RotateAvatar>();
            if (dataStore.AvatarProperties.BodyType == BodyType.FullBody)
            {
                avatar.GetComponent<Animator>().runtimeAnimatorController = animator;
            }
        }
    }
}
