using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// For downloading and filtering all partner assets.
    /// </summary>
    public static class PartnerAssetsManager
    {
        public static async Task<Dictionary<PartnerAsset, Task<Texture>>> GetAllAssets(string token, string partner, BodyType bodyType, OutfitGender gender)
        {
            var assets = await PartnerAssetsRequests.Get(token, partner);
            assets = assets.Where(asset => FilterAssets(asset, bodyType, gender)).ToArray();

            var assetIconDownloadTasks = new Dictionary<PartnerAsset, Task<Texture>>();

            foreach (var asset in assets)
            {
                var iconDownloadTask = PartnerAssetsRequests.GetAssetIcon(
                    token,
                    asset.AssetType == AssetType.EyeColor ? asset.Mask + "?w=256" : asset.Icon);
                assetIconDownloadTasks.Add(asset, iconDownloadTask);
            }

            return assetIconDownloadTasks;
        }

        private static bool FilterAssets(PartnerAsset asset, BodyType bodyType, OutfitGender gender)
        {
            // Outfit is only for full body and Shirt is only for half body.
            // Both outfit and shirt are gender specific.
            if (bodyType == BodyType.HalfBody)
            {
                if (asset.AssetType == AssetType.Shirt)
                    return asset.Gender == gender;

                return asset.AssetType != AssetType.Outfit;
            }

            if (asset.AssetType == AssetType.Outfit)
                return asset.Gender == gender;

            return asset.AssetType != AssetType.Shirt;
        }
    }
}
