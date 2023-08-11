using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// For downloading and filtering all partner assets.
    /// </summary>
    public class PartnerAssetsManager : IDisposable
    {
        private const string EYE_MASK_SIZE_PARAM = "?w=256";

        private readonly BodyType bodyType;
        private readonly OutfitGender gender;

        private readonly PartnerAssetsRequests partnerAssetsRequests;
        private readonly CancellationTokenSource ctxSource;
        private readonly Dictionary<AssetType, List<PartnerAsset>> assetsByType;
        public Action<string> OnError { get; set; }

        private PartnerAsset[] assets;

        public PartnerAssetsManager(BodyType bodyType, OutfitGender gender, CancellationToken token = default)
        {
            this.bodyType = bodyType;
            this.gender = gender;
            ctxSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            partnerAssetsRequests = new PartnerAssetsRequests();
            assetsByType = new Dictionary<AssetType, List<PartnerAsset>>();
        }

        public async Task<List<string>> GetAssetsByCategory(AssetType type)
        {
            assets = await partnerAssetsRequests.Get(type, ctxSource.Token);
            if (assetsByType.TryGetValue(type, out List<PartnerAsset> value))
            {
                value.AddRange(assets);
            }
            else
            {
                assetsByType.Add(type, assets.ToList());
            }

            return assets.Select(x => x.Id).ToList();
        }

        public async Task<Dictionary<string, AssetType>> GetAllAssets()
        {
            try
            {
                assets = await partnerAssetsRequests.Get(ctxSource.Token);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }

            assets = assets.Where(FilterAssets).ToArray();
            return assets.ToDictionary(asset => asset.Id, asset => asset.AssetType);
        }

        public async void DownloadAssetsIcon(AssetType assetType, Action<string, Texture> onDownload)
        {
            var startTime = Time.time;
            var chunkList = assetsByType[assetType].ChunkBy(20);

            foreach (var list in chunkList)
            {
                try
                {
                    await DownloadIcons(list, onDownload);
                    Debug.Log("Download first chunk of icons: " + (Time.time - startTime) + "s");
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e.Message);
                    return;
                }

                if (ctxSource.IsCancellationRequested)
                {
                    return;
                }
                await Task.Yield();
            }
        }

        public bool IsLockedAssetCategories(string id)
        {
            var asset = assets.FirstOrDefault(x => x.Id == id);
            return asset.LockedCategories != null && asset.LockedCategories.Length > 0;
        }

        private async Task DownloadIcons(List<PartnerAsset> chunk, Action<string, Texture> onDownload)
        {
            var assetIconMap = new Dictionary<string, Task<Texture>>();

            foreach (var asset in chunk)
            {
                var url = asset.AssetType == AssetType.EyeColor ? asset.Mask + EYE_MASK_SIZE_PARAM : asset.Icon + "?w=64";
                var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ctxSource.Token);
                var iconTask = partnerAssetsRequests.GetAssetIcon(url,icon =>
                {
                    onDownload?.Invoke(asset.Id, icon);
                },
                    
                linkedTokenSource.Token);
                assetIconMap.Add(asset.Id, iconTask);
            }

            while (!assetIconMap.Values.All(x => x.IsCompleted) && !ctxSource.IsCancellationRequested)
            {
                await Task.Yield();
            }
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

        public void Dispose()
        {
            ctxSource?.Cancel();
        }
    }
}
