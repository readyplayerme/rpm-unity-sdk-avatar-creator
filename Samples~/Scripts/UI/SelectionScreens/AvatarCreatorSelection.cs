using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class AvatarCreatorSelection : State, IDisposable
    {
        private const string UPDATING_YOUR_AVATAR_LOADING_TEXT = "Updating your avatar";

        [SerializeField] private AssetTypeUICreator assetTypeUICreator;
        [SerializeField] private AssetButtonCreator assetButtonCreator;
        [SerializeField] private Button saveButton;
        [SerializeField] private AvatarConfig inCreatorConfig;
        [SerializeField] private RuntimeAnimatorController animator;
        [SerializeField] private AccountCreationPopup accountCreationPopup;

        private PartnerAssetsManager partnerAssetManager;
        private AvatarManager avatarManager;

        private GameObject currentAvatar;
        private Quaternion lastRotation;

        private CancellationTokenSource ctxSource;

        public override StateType StateType => StateType.Editor;
        public override StateType NextState => StateType.End;

        private void Start()
        {
            partnerAssetManager = new PartnerAssetsManager();
        }

        public override void ActivateState()
        {
            saveButton.onClick.AddListener(OnSaveButton);
            accountCreationPopup.OnSendEmail += OnSendEmail;
            accountCreationPopup.OnContinueWithoutSignup += Save;
            assetTypeUICreator.OnCategorySelected += OnCategorySelected;
            Setup();
        }

        public override void DeactivateState()
        {
            saveButton.onClick.RemoveListener(OnSaveButton);
            accountCreationPopup.OnSendEmail -= OnSendEmail;
            accountCreationPopup.OnContinueWithoutSignup -= Save;
            assetTypeUICreator.OnCategorySelected -= OnCategorySelected;
            Cleanup();
        }

        private async void Setup()
        {
            LoadingManager.EnableLoading();
            ctxSource = new CancellationTokenSource();

            avatarManager = new AvatarManager(
                AvatarCreatorData.AvatarProperties.BodyType,
                AvatarCreatorData.AvatarProperties.Gender,
                inCreatorConfig,
                ctxSource.Token);
            avatarManager.OnError += OnErrorCallback;

            currentAvatar = await LoadAvatar();

            if (string.IsNullOrEmpty(avatarManager.AvatarId))
                return;

            await LoadAssets();
            await LoadAvatarColors();
            ToggleAssetTypeButtons();

            LoadingManager.DisableLoading();
        }

        private void ToggleAssetTypeButtons()
        {
            var assets = AvatarCreatorData.AvatarProperties.Assets;
            if (!assets.TryGetValue(AssetType.Outfit, out var outfitId))
            {
                return;
            }
            
            if (partnerAssetManager.IsLockedAssetCategories(outfitId.ToString()))
            {
                assetTypeUICreator.SetActiveAssetTypeButtons(false);
                assetTypeUICreator.SetDefaultSelection(AssetType.Outfit);
            }
            else
            {
                assetTypeUICreator.SetActiveAssetTypeButtons(true);
            }
        }
        
        private void Cleanup()
        {
            if (currentAvatar != null)
            {
                Destroy(currentAvatar);
            }

            avatarManager.DeleteDraft();
            partnerAssetManager.DeleteAssets();

            Dispose();
            assetTypeUICreator.ResetUI();
        }

        private void OnErrorCallback(string error)
        {
            Debug.Log(error);
            avatarManager.OnError -= OnErrorCallback;
            partnerAssetManager.OnError -= OnErrorCallback;

            ctxSource?.Cancel();
            StateMachine.GoToPreviousState();
            LoadingManager.EnableLoading(error, LoadingManager.LoadingType.Popup, false);
        }

        private async Task LoadAssets()
        {
            var startTime = Time.time;
            partnerAssetManager.SetAvatarProperties(
                AvatarCreatorData.AvatarProperties.BodyType,
                AvatarCreatorData.AvatarProperties.Gender,
                ctxSource.Token);

            partnerAssetManager.OnError += OnErrorCallback;
            Debug.Log("Getting all partner assets");

            CreateUI(AvatarCreatorData.AvatarProperties.BodyType);

            await CreateAssetsByCategory(AssetType.FaceShape);

            DebugPanel.AddLogWithDuration("Got all partner assets", Time.time - startTime);
        }

        private async void OnCategorySelected(AssetType category)
        {
            Debug.Log(category);
            await CreateAssetsByCategory(category);
        }

        private async Task<GameObject> LoadAvatar()
        {
            var startTime = Time.time;

            GameObject avatar;

            if (string.IsNullOrEmpty(AvatarCreatorData.AvatarProperties.Id))
            {
                AvatarCreatorData.AvatarProperties.Assets ??= GetDefaultAssets();

                AvatarCreatorData.AvatarProperties = await avatarManager.CreateNewAvatar(AvatarCreatorData.AvatarProperties);
                avatar = await avatarManager.GetPreviewAvatar(AvatarCreatorData.AvatarProperties.Id);
            }
            else
            {
                if (!AvatarCreatorData.IsExistingAvatar)
                {
                    AvatarCreatorData.AvatarProperties = await avatarManager.CreateFromTemplateAvatar(AvatarCreatorData.AvatarProperties);
                    avatar = await avatarManager.GetPreviewAvatar(AvatarCreatorData.AvatarProperties.Id);
                }
                else
                {
                    avatar = await avatarManager.GetAvatar(AvatarCreatorData.AvatarProperties.Id);
                }
            }

            if (avatar == null)
            {
                return null;
            }

            ProcessAvatar(avatar);

            DebugPanel.AddLogWithDuration("Avatar loaded", Time.time - startTime);
            return avatar;
        }

        private async Task LoadAvatarColors()
        {
            var startTime = Time.time;
            var colors = await avatarManager.LoadAvatarColors();
            assetButtonCreator.CreateColorUI(colors, UpdateAvatar);
            DebugPanel.AddLogWithDuration("All colors loaded", Time.time - startTime);
        }

        private void CreateUI(BodyType bodyType)
        {
            assetTypeUICreator.CreateUI(bodyType, AssetTypeHelper.GetAssetTypeList(bodyType));
            assetButtonCreator.SetSelectedAssets(AvatarCreatorData.AvatarProperties.Assets);
            assetButtonCreator.CreateClearButton(UpdateAvatar);
            saveButton.gameObject.SetActive(true);
        }

        private async Task CreateAssetsByCategory(AssetType assetType)
        {
            var assets = await partnerAssetManager.GetAssetsByCategory(assetType);
            if (assets == null || assets.Count == 0)
            {
                return;
            }
            assetButtonCreator.CreateAssetButtons(assets, assetType, OnAssetButtonClicked);
            partnerAssetManager.DownloadAssetsIcon(assetType, assetButtonCreator.SetAssetIcon);

            if (assetType == AssetType.EyeShape)
            {
                await CreateAssetsByCategory(AssetType.EyeColor);
            }
        }
        

        private void OnAssetButtonClicked(string id, AssetType assetType)
        {
            assetTypeUICreator.SetActiveAssetTypeButtons(!partnerAssetManager.IsLockedAssetCategories(id));
            UpdateAvatar(id, assetType);
        }

        private void OnSaveButton()
        {
            if (AuthManager.IsSignedIn)
            {
                Save();
            }
            else
            {
                accountCreationPopup.gameObject.SetActive(true);
            }

        }

        private void OnSendEmail(string email)
        {
            AuthManager.Signup(email);
            Save();
        }

        private async void Save()
        {
            LoadingManager.EnableLoading("Saving avatar...", LoadingManager.LoadingType.Popup);

            var startTime = Time.time;
            var avatarId = await avatarManager.Save();
            AvatarCreatorData.AvatarProperties.Id = avatarId;
            DebugPanel.AddLogWithDuration("Avatar saved", Time.time - startTime);
            StateMachine.SetState(StateType.End);
          
            LoadingManager.DisableLoading();
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

        private async void UpdateAvatar(object assetId, AssetType assetType)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);
            lastRotation = currentAvatar.transform.rotation;
            LoadingManager.EnableLoading(UPDATING_YOUR_AVATAR_LOADING_TEXT, LoadingManager.LoadingType.Popup);
            var avatar = await avatarManager.UpdateAsset(assetType, assetId);
            if (avatar == null)
            {
                return;
            }

            ProcessAvatar(avatar);
            currentAvatar = avatar;
            LoadingManager.DisableLoading();
            DebugPanel.AddLogWithDuration("Avatar updated", Time.time - startTime);
        }

        private void ProcessAvatar(GameObject avatar)
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
            partnerAssetManager.OnError -= OnErrorCallback;
            partnerAssetManager?.Dispose();

            avatarManager.OnError -= OnErrorCallback;
            avatarManager?.Dispose();
        }
    }
}
