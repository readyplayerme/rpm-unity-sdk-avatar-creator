using System.Threading.Tasks;
using NativeAvatarCreator;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AvatarCreator : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;
        [SerializeField] private AvatarCreatorSelection avatarCreatorSelection;
        [SerializeField] private AvatarLoader avatarLoader;

        private string avatarId;

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
            await CreateDefaultModel();
        }

        private async Task CreateDefaultModel()
        {
            dataStore.Payload.Assets = new PayloadAssets
            {
                SkinColor = 5,
                EyeColor = "9781796",
                HairStyle = "23368535",
                EyebrowStyle = "41308196",
                Shirt = "9247449",
                Outfit = "109373713"
            };

            avatarId = await AvatarAPIRequests.Create(dataStore.User.Token, dataStore.Payload);

            var data = await AvatarAPIRequests.GetPreviewAvatar(dataStore.User.Token, avatarId);
            await avatarLoader.LoadAvatar(avatarId, data);
        }

        public async void UpdateAvatar(string assetId, string assetType)
        {
            var payload = new Payload
            {
                Assets = new PayloadAssets()
            };

            switch (assetType)
            {
                case "outfit":
                    payload.Assets.Outfit = assetId;
                    break;
                case "headwear":
                    payload.Assets.Headwear = assetId;
                    break;
                case "beard":
                    payload.Assets.BeardStyle = assetId;
                    break;
                case "eyeShape":
                    payload.Assets.EyeShape = assetId;
                    break;
                case "eyebrowStyle":
                    payload.Assets.EyebrowStyle = assetId;
                    break;
                case "faceMask":
                    payload.Assets.FaceMask = assetId;
                    break;
                case "faceShape":
                    payload.Assets.FaceShape = assetId;
                    break;
                case "glasses":
                    payload.Assets.Glasses = assetId;
                    break;
                case "hair":
                    payload.Assets.HairStyle = assetId;
                    break;
                case "lipShape":
                    payload.Assets.LipShape = assetId;
                    break;
                case "noseShape":
                    payload.Assets.NoseShape = assetId;
                    break;
                case "shirt":
                    payload.Assets.Shirt = assetId;
                    break;
            }

            var data = await AvatarAPIRequests.UpdateAvatar(dataStore.User.Token, avatarId, payload);
            await avatarLoader.LoadAvatar(avatarId, data);
        }
    }
}
