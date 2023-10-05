using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// For downloading and filtering all partner assets.
    /// </summary>
    public class PartnerAssetsManager : IDisposable
    {
        private const string TAG = nameof(PartnerAssetsManager);
        private const string EYE_MASK_SIZE_SIZE = "?w=256";
        private const string ASSET_ICON_SIZE = "?w=64";

        private readonly PartnerAssetsRequests partnerAssetsRequests;

        private Dictionary<Category, List<PartnerAsset>> assetsByCategory;
        public Action<string> OnError { get; set; }

        public PartnerAssetsManager()
        {
            partnerAssetsRequests = new PartnerAssetsRequests(CoreSettingsHandler.CoreSettings.AppId);
            assetsByCategory = new Dictionary<Category, List<PartnerAsset>>();
        }

        public async Task<Dictionary<Category,List<PartnerAsset>>> GetAssets(BodyType bodyType, OutfitGender gender, CancellationToken token = default)
        {
            var startTime = Time.time;

            var assets = await partnerAssetsRequests.Get(bodyType, gender, token);

            assetsByCategory = assets.GroupBy(asset => asset.Category).ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

            if (assets.Length != 0)
            {
                SDKLogger.Log(TAG, $"All assets received: {Time.time - startTime:F2}s");
            }

            return assetsByCategory;
        }

        public List<string> GetAssetsByCategory(Category category)
        {
            return assetsByCategory.TryGetValue(category, out List<PartnerAsset> _) ? assetsByCategory[category].Select(x => x.Id).ToList() : new List<string>();
        }

        public async Task DownloadIconsByCategory(Category category, Action<string, Texture> onDownload, CancellationToken token = default)
        {
            var startTime = Time.time;
            var chunkList = assetsByCategory[category].ChunkBy(20);

            foreach (var list in chunkList)
            {
                try
                {
                    await DownloadIcons(list, onDownload, token);
                    SDKLogger.Log(TAG, $"Download chunk of {category} icons: " + (Time.time - startTime) + "s");
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e.Message);
                    return;
                }

                if (token.IsCancellationRequested)
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

        private async Task DownloadIcons(List<PartnerAsset> chunk, Action<string, Texture> onDownload, CancellationToken token = default)
        {
            var assetIconMap = new Dictionary<string, Task<Texture>>();

            foreach (var asset in chunk)
            {
                var url = asset.Category == Category.EyeColor ? asset.Mask + EYE_MASK_SIZE_SIZE : asset.Icon + ASSET_ICON_SIZE;
                var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                var iconTask = partnerAssetsRequests.GetAssetIcon(url, icon =>
                    {
                        onDownload?.Invoke(asset.Id, icon);
                    },
                    linkedTokenSource.Token);
                assetIconMap.Add(asset.Id, iconTask);
            }

            while (!assetIconMap.Values.All(x => x.IsCompleted) && !token.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }

        public void DeleteAssets()
        {
            assetsByCategory.Clear();
        }

        public void Dispose() => DeleteAssets();

        public PrecompileData GetPrecompileData(Category[] categories, int numberOfAssetsPerCategory)
        {
            var categoriesFromMap = CategoryHelper.PartnerCategoryMap
                .Where(kvp => categories.Contains(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToArray();

            var dictionary = categoriesFromMap.ToDictionary(category => category, category => 
                GetAssetsByCategory(CategoryHelper.PartnerCategoryMap[category])
                    .Take(numberOfAssetsPerCategory)
                    .ToArray());

            return new PrecompileData { data = dictionary };
        }
    }
}
