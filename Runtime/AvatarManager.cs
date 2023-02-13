using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public class AvatarManager
    {
        private readonly AvatarAPIRequests avatarAPIRequests;
        private readonly string avatarConfigParameters;
        private readonly InCreatorAvatarLoader inCreatorAvatarLoader;

        private string avatarId;
        
        public AvatarManager(string token, AvatarConfig avatarConfig)
        {
            avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
            inCreatorAvatarLoader = new InCreatorAvatarLoader();
            avatarAPIRequests = new AvatarAPIRequests(token);
        }

        public async Task<GameObject> Create(AvatarProperties avatarProperties)
        {
            avatarId = await avatarAPIRequests.Create(avatarProperties);
            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId, avatarConfigParameters);
            return await inCreatorAvatarLoader.Load(avatarId, avatarProperties.BodyType, avatarProperties.Gender, data);
        }

        public async Task<GameObject> Update(BodyType bodyType, OutfitGender gender, string assetId, AssetType assetType)
        {
            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);

            var data = await avatarAPIRequests.UpdateAvatar(avatarId, payload, avatarConfigParameters);
            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        public async Task<string> Save()
        {
            await avatarAPIRequests.SaveAvatar(avatarId);
            return avatarId;
        }
    }
}
