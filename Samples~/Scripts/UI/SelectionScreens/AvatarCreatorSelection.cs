using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorSelection : State, IDisposable
    {
        [SerializeField] private AssetTypeUICreator assetTypeUICreator;
        [SerializeField] private AssetButtonCreator assetButtonCreator;
        [SerializeField] private Button saveButton;
        [SerializeField] private AvatarConfig inCreatorConfig;
        [SerializeField] private RuntimeAnimatorController animator;

        private PartnerAssetsManager partnerAssetManager;
        private AvatarManager avatarManager;
        private GameObject avatar;
        private Quaternion lastRotation;

        public override StateType StateType => StateType.Editor;
        public override StateType NextState => StateType.End;

        private void OnEnable()
        {
            saveButton.onClick.AddListener(OnSave);
            Loading.SetActive(true);
            Initialize();
        }

        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(OnSave);

            if (avatar != null)
            {
                Destroy(avatar);
            }
            saveButton.gameObject.SetActive(false);

            Dispose();
            assetTypeUICreator.ResetUI();
        }

        private async void Initialize()
        {
            if (!AuthManager.IsSignedIn)
            {
                await AuthManager.LoginAsAnonymous();
            }
            LoadAssets();
            CreateDefaultModel();
        }

        private void CreateUI(BodyType bodyType, Dictionary<string, AssetType> assets)
        {
            assetTypeUICreator.CreateUI(bodyType, AssetTypeHelper.GetAssetTypeList(bodyType));
            assetButtonCreator.CreateUI(assets, UpdateAvatar);
            saveButton.gameObject.SetActive(true);
        }

        private async void OnSave()
        {
            var startTime = Time.time;
            var avatarId = await avatarManager.Save();
            AvatarCreatorData.AvatarId = avatarId;
            DebugPanel.AddLogWithDuration("Avatar saved", Time.time - startTime);
            StateMachine.SetState(StateType.End);
        }

        private async void LoadAssets()
        {
            var startTime = Time.time;

            AvatarCreatorData.AvatarProperties.Partner = CoreSettingsHandler.CoreSettings.Subdomain;

            partnerAssetManager = new PartnerAssetsManager(
                AvatarCreatorData.AvatarProperties.Partner,
                AvatarCreatorData.AvatarProperties.BodyType,
                AvatarCreatorData.AvatarProperties.Gender);
            var assetIconDownloadTasks = await partnerAssetManager.GetAllAssets();

            DebugPanel.AddLogWithDuration("Got all partner assets", Time.time - startTime);
            CreateUI(AvatarCreatorData.AvatarProperties.BodyType, assetIconDownloadTasks);
            partnerAssetManager.DownloadAssetsIcon(assetButtonCreator.SetAssetIcons);
        }

        private async Task<ColorPalette[]> LoadColors()
        {
            var startTime = Time.time;
            var avatarAPIRequests = new AvatarAPIRequests();
            DebugPanel.AddLogWithDuration("All colors loaded", Time.time - startTime);
            return await avatarAPIRequests.GetAllAvatarColors(avatar.name); // avatar.name is same as draft avatar ID
        }

        private async void CreateDefaultModel()
        {
            var startTime = Time.time;

            AvatarCreatorData.AvatarProperties.Assets = GetDefaultAssets();
            
            avatarManager = new AvatarManager(
                AvatarCreatorData.AvatarProperties.BodyType,
                AvatarCreatorData.AvatarProperties.Gender,
                inCreatorConfig);

            avatar = await avatarManager.Create(AvatarCreatorData.AvatarProperties);
            if (avatar == null)
            {
                return;
            }

            DebugPanel.AddLogWithDuration("Avatar loaded", Time.time - startTime);
            assetButtonCreator.CreateColorUI(await LoadColors(), UpdateAvatarColor);
            ProcessAvatar();
            Loading.SetActive(false);
        }

        private Dictionary<AssetType, object> GetDefaultAssets()
        {
            if (string.IsNullOrEmpty(AvatarCreatorData.AvatarProperties.Base64Image))
            {
                return AvatarCreatorData.AvatarProperties.Gender == OutfitGender.Feminine
                    ? AvatarPropertiesConstants.FemaleDefaultAssets
                    : AvatarPropertiesConstants.MaleDefaultAssets;
            }
            
            return new Dictionary<AssetType, object>();
        }
        
        private async void UpdateAvatarColor(AssetType assetType, int assetIndex)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetIndex);
            lastRotation = avatar.transform.rotation = lastRotation;
            avatar = await avatarManager.UpdateAsset(assetType, assetIndex);
            if (avatar == null)
            {
                return;
            }

            ProcessAvatar();
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        private async void UpdateAvatar(string assetId, AssetType assetType)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);
            lastRotation = avatar.transform.rotation = lastRotation;
            avatar = await avatarManager.UpdateAsset(assetType, assetId);
            if (avatar == null)
            {
                return;
            }

            ProcessAvatar();
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }


        private void ProcessAvatar()
        {
            if (AvatarCreatorData.AvatarProperties.BodyType == BodyType.FullBody)
            {
                avatar.GetComponent<Animator>().runtimeAnimatorController = animator;
            }
            avatar.transform.rotation = lastRotation;
            avatar.AddComponent<RotateAvatar>();
        }

        public void Dispose()
        {
            partnerAssetManager?.Dispose();
            avatarManager?.Dispose();
        }
    }
}
