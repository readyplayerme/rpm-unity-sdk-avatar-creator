using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// It is responsible for creating a new avatar, updating and deleting an avatar.
    /// </summary>
    public class AvatarManager
    {
        private readonly BodyType bodyType;
        private readonly OutfitGender gender;
        private readonly AvatarAPIRequests avatarAPIRequests;
        private readonly string avatarConfigParameters;
        private readonly InCreatorAvatarLoader inCreatorAvatarLoader;

        private string avatarId;

        /// <param name="token">Authentication token</param>
        /// <param name="bodyType">Body type of avatar</param>
        /// <param name="gender">Gender of avatar</param>
        /// <param name="avatarConfig">Config for downloading preview avatar</param>
        public AvatarManager(string token, BodyType bodyType, OutfitGender gender, AvatarConfig avatarConfig = null)
        {
            this.bodyType = bodyType;
            this.gender = gender;

            if (avatarConfig != null)
            {
                avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
            }

            inCreatorAvatarLoader = new InCreatorAvatarLoader();
            avatarAPIRequests = new AvatarAPIRequests(token);
        }

        public async Task<GameObject> Create(AvatarProperties avatarProperties)
        {
            avatarId = await avatarAPIRequests.CreateNewAvatar(avatarProperties);
            var data = await avatarAPIRequests.GetPreviewAvatar(avatarId, avatarConfigParameters);
            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        /// <summary>
        /// Update an asset of the avatar.
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public async Task<GameObject> Update(string assetId, AssetType assetType)
        {
            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);

            var data = await avatarAPIRequests.UpdateAvatar(avatarId, payload, avatarConfigParameters);
            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        /// <summary>
        /// Saves the avatar from temp to permanent storage. 
        /// </summary>
        public async Task<string> Save()
        {
            await avatarAPIRequests.SaveAvatar(avatarId);
            return avatarId;
        }

        /// <summary>
        /// This will delete the avatar completely even from database. 
        /// </summary>
        public async Task Delete()
        {
            await avatarAPIRequests.DeleteAvatar(avatarId);
        }
    }
}
