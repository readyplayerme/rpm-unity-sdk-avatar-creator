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
            var startTime = Time.time;
            var assets = await PartnerAssetsRequests.Get(dataStore.User.Token, dataStore.Payload.Partner);
            assets = assets.Where(FilterAssets).ToArray();
            DebugPanel.AddLogWithDuration("Got all partner assets", Time.time - startTime);

            var assetIconDownloadTasks = new Dictionary<PartnerAsset, Task<Texture>>();

            foreach (var asset in assets)
            {
                var iconDownloadTask = PartnerAssetsRequests.GetAssetIcon(dataStore.User.Token, asset.Icon);
                assetIconDownloadTasks.Add(asset, iconDownloadTask);
            }

            avatarCreatorSelection.InstantNoodles(assetIconDownloadTasks, avatarCreator.UpdateAvatar);
            await Task.Yield();
        }

        private bool FilterAssets(PartnerAsset asset)
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
