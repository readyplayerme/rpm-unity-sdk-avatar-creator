using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    [Serializable]
    public struct PrecompileData
    {
        public Dictionary<string, string[]> data;
    }
    /// <summary>
    /// For downloading and filtering all partner assets.
    /// </summary>
    public class PartnerAssetsManager : IDisposable
    {
        private const string TAG = nameof(PartnerAssetsManager);
        private const string EYE_MASK_SIZE_SIZE = "?w=256";
        private const string ASSET_ICON_SIZE = "?w=64";

        private readonly PartnerAssetsRequests partnerAssetsRequests;
        private readonly Dictionary<Category, List<PartnerAsset>> assetsByCategory;
        public Action<string> OnError { get; set; }

        private BodyType bodyType;
        private OutfitGender gender;
        private CancellationTokenSource ctxSource;

        public PrecompileData GetPrecompileData(Category[] categories)
        {
            var dictionary = categories.ToDictionary(category => category.ToString().ToLowerInvariant(), category => GetAssetsByCategory(category).ToArray().Take(20).ToArray());
            return new PrecompileData { data = dictionary };
        }

        public PartnerAssetsManager()
        {
            partnerAssetsRequests = new PartnerAssetsRequests(CoreSettingsHandler.CoreSettings.AppId);
            assetsByCategory = new Dictionary<Category, List<PartnerAsset>>();
        }

        public void SetAvatarProperties(BodyType assetBodyType, OutfitGender assetGender, CancellationToken token = default)
        {
            bodyType = assetBodyType;
            gender = assetGender;
            ctxSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        }

        public async Task GetAssets()
        {
            var startTime = Time.time;

            var assets = await partnerAssetsRequests.Get(bodyType, gender, ctxSource.Token);
            foreach (PartnerAsset asset in assets)
            {
                if (assetsByCategory.TryGetValue(asset.Category, out List<PartnerAsset> value))
                {
                    value.Add(asset);
                }
                else
                {
                    assetsByCategory.Add(asset.Category, new List<PartnerAsset> { asset });
                }
            }

            if (assets.Length != 0)
            {
                SDKLogger.Log(TAG, $"All asset received: {Time.time - startTime:F2}s");
            }
        }

        public List<string> GetAssetsByCategory(Category category)
        {
            return assetsByCategory.TryGetValue(category, out List<PartnerAsset> _) ? assetsByCategory[category].Select(x => x.Id).ToList() : new List<string>();
        }

        public async Task DownloadAssetsIcon(Category category, Action<string, Texture> onDownload)
        {
            var startTime = Time.time;
            var chunkList = assetsByCategory[category].ChunkBy(20);

            foreach (var list in chunkList)
            {
                try
                {
                    await DownloadIcons(list, onDownload);
                    SDKLogger.Log(TAG, $"Download chunk of {category} icons: " + (Time.time - startTime) + "s");
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

        public bool IsLockedAssetCategories(Category category, string id)
        {
            if (!assetsByCategory.ContainsKey(category))
            {
                return false;
            }

            var asset = assetsByCategory[category].FirstOrDefault(x => x.Id == id);
            return asset.LockedCategories != null && asset.LockedCategories.Length > 0;
        }

        private async Task DownloadIcons(List<PartnerAsset> chunk, Action<string, Texture> onDownload)
        {
            var assetIconMap = new Dictionary<string, Task<Texture>>();

            foreach (var asset in chunk)
            {
                var url = asset.Category == Category.EyeColor ? asset.Mask + EYE_MASK_SIZE_SIZE : asset.Icon + ASSET_ICON_SIZE;
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
            assetsByCategory.Clear();
            ctxSource?.Cancel();
        }

        public void Dispose() => DeleteAssets();
    }
}
