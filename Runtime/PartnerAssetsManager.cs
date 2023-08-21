using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;
using Random = UnityEngine.Random;

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


        public Dictionary<Category, float> probabilityToChangeByCategory = new Dictionary<Category, float>()
        {
            { Category.FaceShape, 1 },
            { Category.EyeShape, 1 },
            { Category.EyeColor, 1 },
            { Category.EyebrowStyle, 1 },
            { Category.NoseShape, 1 },
            { Category.LipShape, 1 },
            { Category.BeardStyle, 1 },
            { Category.HairStyle, 1 },
            { Category.Outfit, 1 },
            { Category.Shirt, 1 },
            { Category.Glasses, 0.3f },
            { Category.FaceMask, 0.1f },
            { Category.Facewear, 0.4f },
            { Category.Headwear, 0.1f }
        };

        public Dictionary<Category, object> GetRandomAssets()
        {
            var assets = new Dictionary<Category, object>();
            foreach (var category in assetsByCategory)
            {
                if (category.Key == Category.BeardStyle && gender == OutfitGender.Feminine)
                {
                    continue;
                }

                var shouldChange = Random.Range(0f, 1f);

                var value = probabilityToChangeByCategory[category.Key];
                if (value <= 0.9f && shouldChange > probabilityToChangeByCategory[category.Key])
                {
                    continue;
                }
                    
                var randomValue = Random.Range(0, category.Value.Count);
                var randomAsset = category.Value[randomValue];
                assets.Add(category.Key, randomAsset.Id);
            }

            return assets;
        }

        public async Task GetAssets()
        {
            var startTime = Time.time;

            foreach (var category in CategoryHelper.AssetAPISupportedCategory)
            {
                var assets = await partnerAssetsRequests.Get(category, bodyType, gender, ctxSource.Token);
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

        public bool IsLockedAssetCategories(Category category, string id)
        {
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
