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

        private GameObject avatar;
        private AvatarManager avatarManager;

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
