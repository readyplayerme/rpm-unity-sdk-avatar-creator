using System;
using System.Collections.Generic;
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

        [SerializeField] private CategoryUICreator categoryUICreator;
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
        public List<Category> categoriesAssetsLoaded;

        public override StateType StateType => StateType.Editor;
        public override StateType NextState => StateType.End;

        private void Start()
        {
            partnerAssetManager = new PartnerAssetsManager( "645b4dd53aef3a0696a2b32c");
        }

        public override void ActivateState()
        {
            saveButton.onClick.AddListener(OnSaveButton);
            accountCreationPopup.OnSendEmail += OnSendEmail;
            accountCreationPopup.OnContinueWithoutSignup += Save;
            categoryUICreator.OnCategorySelected += OnCategorySelected;
            Setup();
        }

        public override void DeactivateState()
        {
            saveButton.onClick.RemoveListener(OnSaveButton);
            accountCreationPopup.OnSendEmail -= OnSendEmail;
            accountCreationPopup.OnContinueWithoutSignup -= Save;
            categoryUICreator.OnCategorySelected -= OnCategorySelected;
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
            ToggleCategoryButtons();

            LoadingManager.DisableLoading();
        }

        private void ToggleCategoryButtons()
        {
            var assets = AvatarCreatorData.AvatarProperties.Assets;
            if (!assets.TryGetValue(Category.Outfit, out var outfitId))
            {
                return;
            }
            
            if (partnerAssetManager.IsLockedAssetCategories(outfitId.ToString()))
            {
                categoryUICreator.SetActiveCategoryButtons(false);
                categoryUICreator.SetDefaultSelection(Category.Outfit);
            }
            else
            {
                categoryUICreator.SetActiveCategoryButtons(true);
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
            categoryUICreator.ResetUI();
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

            CreateUI(AvatarCreatorData.AvatarProperties.BodyType);
            categoriesAssetsLoaded = new List<Category>();
            
            await partnerAssetManager.GetAssets();
            await CreateAssetsByCategory(Category.FaceShape);

            DebugPanel.AddLogWithDuration("Got all partner assets", Time.time - startTime);
        }

        private async void OnCategorySelected(Category category)
        {
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
            categoryUICreator.CreateUI(bodyType, CategoryHelper.GetCategories(bodyType));
            assetButtonCreator.SetSelectedAssets(AvatarCreatorData.AvatarProperties.Assets);
            assetButtonCreator.CreateClearButton(UpdateAvatar);
            saveButton.gameObject.SetActive(true);
        }


        private async Task CreateAssetsByCategory(Category category)
        {
            if(!categoriesAssetsLoaded.Contains(category))
            {
                categoriesAssetsLoaded.Add(category);
            }
            else
            {
                return;
            }

            var assets = partnerAssetManager.GetAssetsByCategory(category);
            if (assets == null || assets.Count == 0)
            {
                return;
            }
            assetButtonCreator.CreateAssetButtons(assets, category, OnAssetButtonClicked);
            await partnerAssetManager.DownloadAssetsIcon(category, assetButtonCreator.SetAssetIcon);

            if (category == Category.EyeShape)
            {
                await CreateAssetsByCategory(Category.EyeColor);
            }
        }

        private void OnAssetButtonClicked(string id, Category category)
        {
            categoryUICreator.SetActiveCategoryButtons(!partnerAssetManager.IsLockedAssetCategories(id));
            UpdateAvatar(id, category);
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

        private Dictionary<Category, object> GetDefaultAssets()
        {
            if (string.IsNullOrEmpty(AvatarCreatorData.AvatarProperties.Base64Image))
            {
                return AvatarCreatorData.AvatarProperties.Gender == OutfitGender.Feminine
                    ? AvatarPropertiesConstants.FemaleDefaultAssets
                    : AvatarPropertiesConstants.MaleDefaultAssets;
            }

            return new Dictionary<Category, object>();
        }

        private async void UpdateAvatar(object assetId, Category category)
        {
            var startTime = Time.time;

            var payload = new AvatarProperties
            {
                Assets = new Dictionary<Category, object>()
            };

            payload.Assets.Add(category, assetId);
            lastRotation = currentAvatar.transform.rotation;
            LoadingManager.EnableLoading(UPDATING_YOUR_AVATAR_LOADING_TEXT, LoadingManager.LoadingType.Popup);
            var avatar = await avatarManager.UpdateAsset(category, assetId);
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
