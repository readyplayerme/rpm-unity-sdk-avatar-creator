using System;
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
    public class PartnerAssetsManager
    {
        private const string EYE_MASK_SIZE_PARAM = "?w=256";
        
        private readonly string token;
        private readonly string partner;
        private readonly BodyType bodyType;
        private readonly OutfitGender gender;

        private PartnerAsset[] assets;

        public PartnerAssetsManager(string token, string partner, BodyType bodyType, OutfitGender gender)
        {
            this.token = token;
            this.partner = partner;
            this.bodyType = bodyType;
            this.gender = gender;
        }

        public async Task<Dictionary<string, AssetType>> GetAllAssets()
        {
            assets = await PartnerAssetsRequests.Get(token, partner);
            assets = assets.Where(FilterAssets).ToArray();
            return assets.ToDictionary(asset => asset.Id, asset => asset.AssetType);
        }

        public async void DownloadAssetsIcon(Action<Dictionary<string, Texture>> onDownload)
        {
            var ordererAssets = assets.OrderByDescending(x => x.AssetType == AssetType.FaceShape);
            var chunkList = ordererAssets.ChunkBy(20);

            foreach (var list in chunkList)
            {
                var assetIdTextureMap = await DownloadIcons(list);
                onDownload?.Invoke(assetIdTextureMap);
                await Task.Yield();
            }
        }

        private async Task<Dictionary<string, Texture>> DownloadIcons(List<PartnerAsset> chunk)
        {
            var assetIconMap = new Dictionary<string, Task<Texture>>();

            foreach (var asset in chunk)
            {
                var url = asset.AssetType == AssetType.EyeColor ? asset.Mask + EYE_MASK_SIZE_PARAM : asset.Icon;
                var iconTask = PartnerAssetsRequests.GetAssetIcon(token, url);
                assetIconMap.Add(asset.Id, iconTask);
            }

            while (!assetIconMap.Values.All(x => x.IsCompleted))
            {
                await Task.Yield();
            }

            return assetIconMap.ToDictionary(a => a.Key, a => a.Value.Result);
        }

        private bool FilterAssets(PartnerAsset asset)
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
