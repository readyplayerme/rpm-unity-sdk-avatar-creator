using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class PartnerAssetManager : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;

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
            await CreateDefaultModel();
        }

        private async Task GetAllAssets()
        {
            var assets = await PartnerAssetsRequests.Get(dataStore.User.Token, dataStore.Payload.Partner);

            var assetsByGenderType = assets
                .Where(asset => (asset.Gender == dataStore.Payload.Gender || asset.AssetType != "outfit") && asset.AssetType != "shirt").ToList();

            var assetIcons = new Dictionary<PartnerAsset, Texture>();
            foreach (var asset in assetsByGenderType)
            {
                var texture = await PartnerAssetsRequests.GetAssetIcon(dataStore.User.Token, asset.Icon);
                assetIcons.Add(asset, texture);
            }

            avatarCreatorSelection.InstantNoodles(assetIcons);
            await Task.Yield();
        }

        private async Task CreateDefaultModel()
        {
            var avatarAPIRequests = new AvatarAPIRequests(dataStore.User.Token);
            dataStore.Payload.Assets = new PayloadAssets
            {
                SkinColor = 5,
                EyeColor = "9781796",
                HairStyle = "23368535",
                EyebrowStyle = "41308196",
                Shirt = "9247449",
                Outfit = "109373713"
            };

            // var response = await avatarAPIRequests.Create(dataHolder.Payload);
            // Debug.Log(response);
        }
    }
}
