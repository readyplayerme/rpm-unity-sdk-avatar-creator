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

        private readonly PartnerAssetsRequests partnerAssetsRequests;
        private readonly Dictionary<AssetType, List<PartnerAsset>> assetsByType;
        public Action<string> OnError { get; set; }

        private BodyType bodyType;
        private OutfitGender gender;
        private PartnerAsset[] assets;
        private CancellationTokenSource ctxSource;

        public PartnerAssetsManager()
        {
            partnerAssetsRequests = new PartnerAssetsRequests();
            assetsByType = new Dictionary<AssetType, List<PartnerAsset>>();
        }

        public void SetAvatarProperties(BodyType assetBodyType, OutfitGender assetGender, CancellationToken token = default)
        {
            bodyType = assetBodyType;
            gender = assetGender;
            ctxSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        }

        public async Task<List<string>> GetAssetsByCategory(AssetType type)
        {
            var startTime = Time.time;
            if (assetsByType.TryGetValue(type, out List<PartnerAsset> _))
            {
                return new List<string>();
            }

            assets = await partnerAssetsRequests.Get(type, ctxSource.Token);
            Debug.Log($"Asset by category {type} received: {Time.time - startTime}s");
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

        public async void DownloadAssetsIcon(AssetType assetType, Action<string, Texture> onDownload)
        {
            var startTime = Time.time;
            var chunkList = assetsByType[assetType].ChunkBy(20);

            foreach (var list in chunkList)
            {
                try
                {
                    await DownloadIcons(list, onDownload);
                    Debug.Log($"Download chunk of {assetType} icons: " + (Time.time - startTime) + "s");
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
                var iconTask = partnerAssetsRequests.GetAssetIcon(url, icon =>
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

        public void DeleteAssets()
        {
            assetsByType.Clear();
            ctxSource?.Cancel();
        }
        
        public void Dispose() => DeleteAssets();
    }
}
