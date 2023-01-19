using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AvatarCreator : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private AvatarLoader avatarLoader;

        private string avatarId;
        private GameObject avatar;

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

            dataStore.AvatarProperties.Assets = new Dictionary<AssetTypeData.PartnerAssetType, object>()
            {
                { AssetTypeData.PartnerAssetType.SkinColor, 5 },
                { AssetTypeData.PartnerAssetType.EyeColor, "9781796" },
                { AssetTypeData.PartnerAssetType.HairStyle, "9781796" },
                { AssetTypeData.PartnerAssetType.EyebrowStyle, "9781796" },
                { AssetTypeData.PartnerAssetType.Shirt, "9247449" },
                { AssetTypeData.PartnerAssetType.Outfit, "9781796" }
            };
            foreach (AssetTypeData.PartnerAssetType assetType in Enum.GetValues(typeof(AssetTypeData.PartnerAssetType)))
            {
                if (dataStore.AvatarProperties.Assets.ContainsKey(assetType)) continue;
                if (assetType == AssetTypeData.PartnerAssetType.None)
                    continue;

                if (assetType.ToString().Contains("Color"))
                {
                    dataStore.AvatarProperties.Assets.Add(assetType, 0);
                }
                else
                {
                    dataStore.AvatarProperties.Assets.Add(assetType, null);
                }
            }

            avatarId = await AvatarAPIRequests.Create(dataStore.User.Token, dataStore.AvatarProperties);

            var timeForCreateRequest = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar metadata created in temp storage", timeForCreateRequest);
            startTime = timeForCreateRequest;

            var data = await AvatarAPIRequests.GetPreviewAvatar(dataStore.User.Token, avatarId);
            var timeForGettingPreviewAvatar = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Downloaded preview avatar", timeForGettingPreviewAvatar);
            startTime = timeForGettingPreviewAvatar;

            avatar = await avatarLoader.LoadAvatar(avatarId, data);
            var avatarLoadingTime = Time.time - startTime;
            DebugPanel.AddLogWithDuration("Avatar loaded", avatarLoadingTime);
        }

        public async void UpdateAvatar(string assetId, string assetType)
        {
            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetTypeData.PartnerAssetType, object>()
            };

            payload.Assets.Add(AssetTypeData.PartnerAssetTypeMap[assetType], assetId);

            var data = await AvatarAPIRequests.UpdateAvatar(dataStore.User.Token, avatarId, payload);
            avatar = await avatarLoader.LoadAvatar(avatarId, data);
        }
    }
}
