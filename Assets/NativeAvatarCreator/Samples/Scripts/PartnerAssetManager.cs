using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class PartnerAssetManager : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private AvatarLoader avatarLoader;

        private string avatarId;
        private AvatarAPIRequests avatarAPIRequests;

        private void OnEnable()
        {
            avatarCreatorSelection.Show += Show;
        }

        private void OnDisable()
        {
            avatarCreatorSelection.Show -= Show;
        }

        private async void Show()
        {
            avatarAPIRequests = new AvatarAPIRequests(dataStore.User.Token);

            await GetAllAssets();
            await CreateDefaultModel();
        }

        private async Task GetAllAssets()
        {
            var assets = await PartnerAssetsRequests.Get(dataStore.User.Token, dataStore.Payload.Partner);
            assets = assets.Where(asset => (asset.Gender == dataStore.Payload.Gender || asset.AssetType != "outfit") && asset.AssetType != "shirt")
                .ToArray();

            var assetIconDownloadTasks = new Dictionary<PartnerAsset, Task<Texture>>();

            var time = Time.realtimeSinceStartup;

            foreach (var asset in assets)
            {
                var iconDownloadTask =  PartnerAssetsRequests.GetAssetIcon(dataStore.User.Token, asset.Icon);
                assetIconDownloadTasks.Add(asset, iconDownloadTask);
            }
            Debug.Log("Downloaded asset icons: " + (Time.realtimeSinceStartup - time));

            avatarCreatorSelection.InstantNoodles(assetIconDownloadTasks, UpdateAvatar);
            await Task.Yield();
        }

        private async Task CreateDefaultModel()
        {
            dataStore.Payload.Assets = new PayloadAssets
            {
                SkinColor = 5,
                EyeColor = "9781796",
                HairStyle = "23368535",
                EyebrowStyle = "41308196",
                Shirt = "9247449",
                Outfit = "109373713"
            };

            avatarId = await avatarAPIRequests.Create(dataStore.Payload);

            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId);
            await avatarLoader.LoadAvatar(avatarId, data);
        }

        private async void UpdateAvatar(string assetId, string assetType)
        {
            var payload = new Payload
            {
                Assets = new PayloadAssets()
            };

            switch (assetType)
            {
                case "outfit":
                    payload.Assets.Outfit = assetId;
                    break;
                case "headwear":
                    payload.Assets.Headwear = assetId;
                    break;
                case "beard":
                    payload.Assets.BeardStyle = assetId;
                    break;
                case "eyeShape":
                    payload.Assets.EyeShape = assetId;
                    break;
                case "eyebrowStyle":
                    payload.Assets.EyebrowStyle = assetId;
                    break;
                case "faceMask":
                    payload.Assets.FaceMask = assetId;
                    break;
                case "faceShape":
                    payload.Assets.FaceShape = assetId;
                    break;
                case "glasses":
                    payload.Assets.Glasses = assetId;
                    break;
                case "hair":
                    payload.Assets.HairStyle = assetId;
                    break;
                case "lipShape":
                    payload.Assets.LipShape = assetId;
                    break;
                case "noseShape":
                    payload.Assets.NoseShape = assetId;
                    break;
                case "shirt":
                    payload.Assets.Shirt = assetId;
                    break;
            }

            var data = await avatarAPIRequests.UpdateAvatar(avatarId, payload);
            await avatarLoader.LoadAvatar(avatarId, data);
        }
    }
}
