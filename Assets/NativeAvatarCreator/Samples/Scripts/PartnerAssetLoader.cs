using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class PartnerAssetLoader : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private AvatarCreator avatarCreator;

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
            await GetAllAssets();
        }

        private async Task GetAllAssets()
        {
            var assets = await PartnerAssetsRequests.Get(dataStore.User.Token, dataStore.Payload.Partner);
            assets = assets.Where(ValidateAsset).ToArray();

            var assetIconDownloadTasks = new Dictionary<PartnerAsset, Task<Texture>>();

            var time = Time.realtimeSinceStartup;

            foreach (var asset in assets)
            {
                var iconDownloadTask = PartnerAssetsRequests.GetAssetIcon(dataStore.User.Token, asset.Icon);
                assetIconDownloadTasks.Add(asset, iconDownloadTask);
            }
            Debug.Log("Downloaded asset icons: " + (Time.realtimeSinceStartup - time));

            avatarCreatorSelection.InstantNoodles(assetIconDownloadTasks, avatarCreator.UpdateAvatar);
            await Task.Yield();
        }

        private bool ValidateAsset(PartnerAsset asset)
        {
            // Outfit is only for fullbody and are gender specific.
            // Shirt is only for halfbody.

            if (dataStore.Payload.BodyType != "fullbody")
                return asset.AssetType != "outfit";

            if (asset.AssetType == "outfit")
                return asset.Gender == dataStore.Payload.Gender;

            return asset.AssetType != "shirt";
        }
    }
}
