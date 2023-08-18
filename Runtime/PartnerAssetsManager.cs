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
        private const string EYE_MASK_SIZE_SIZE = "?w=256";
        private const string ASSET_ICON_SIZE = "?w=64";

        private readonly PartnerAssetsRequests partnerAssetsRequests;
        private readonly Dictionary<Category, List<PartnerAsset>> assetsByCategory;
        public Action<string> OnError { get; set; }

        private BodyType bodyType;
        private OutfitGender gender;
        private PartnerAsset[] assets;
        private CancellationTokenSource ctxSource;

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

            foreach (var category in CategoryHelper.AssetAPISupportedCategory)
            {
                assets = await partnerAssetsRequests.Get(category, bodyType, gender, ctxSource.Token);
                if (assetsByCategory.TryGetValue(category, out List<PartnerAsset> value))
                {
                    value.AddRange(assets);
                }
                else
                {
                    assetsByCategory.Add(category, assets.ToList());
                }
            }
            
            Debug.Log($"All asset received: {Time.time - startTime}s");
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
                    Debug.Log($"Download chunk of {category} icons: " + (Time.time - startTime) + "s");
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
