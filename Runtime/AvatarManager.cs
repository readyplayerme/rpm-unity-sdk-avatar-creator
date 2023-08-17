using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// It is responsible for creating a new avatar, updating and deleting an avatar.
    /// </summary>
    public class AvatarManager : IDisposable
    {
        private readonly BodyType bodyType;
        private readonly OutfitGender gender;
        private readonly AvatarAPIRequests avatarAPIRequests;
        private readonly string avatarConfigParameters;
        private readonly InCreatorAvatarLoader inCreatorAvatarLoader;
        private readonly CancellationTokenSource ctxSource;

        public Action<string> OnError { get; set; }

        public string AvatarId => avatarId;

        private string avatarId;

        /// <param name="bodyType">Body type of avatar</param>
        /// <param name="gender">Gender of avatar</param>
        /// <param name="avatarConfig">Config for downloading preview avatar</param>
        /// <param name="token">Cancellation token</param>
        public AvatarManager(BodyType bodyType, OutfitGender gender, AvatarConfig avatarConfig = null, CancellationToken token = default)
        {
            this.bodyType = bodyType;
            this.gender = gender;

            if (avatarConfig != null)
            {
                avatarConfigParameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
            }

            ctxSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            inCreatorAvatarLoader = new InCreatorAvatarLoader();
            avatarAPIRequests = new AvatarAPIRequests(ctxSource.Token);
        }
        
        /// <summary>
        /// Create a new avatar from a provided template.
        /// </summary>
        /// <param name="avatarProperties">Properties which describes avatar</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<AvatarProperties> CreateFromTemplateAvatar(AvatarProperties avatarProperties)
        {
            try
            {
                avatarProperties = await avatarAPIRequests.CreateFromTemplateAvatar(
                    avatarProperties.Id,
                    avatarProperties.Partner,
                    bodyType
                );
                avatarId = avatarProperties.Id;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return avatarProperties;
            }
            
            if (ctxSource.IsCancellationRequested)
            {
                return avatarProperties;
            }

            return avatarProperties;
        }

        /// <summary>
        /// Create a new avatar.
        /// </summary>
        /// <param name="avatarProperties">Properties which describes avatar</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<AvatarProperties> CreateNewAvatar(AvatarProperties avatarProperties)
        {
            try
            {
                avatarProperties = await avatarAPIRequests.CreateNewAvatar(avatarProperties);
                avatarId = avatarProperties.Id;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return avatarProperties;
            }

            if (ctxSource.IsCancellationRequested)
            {
                return avatarProperties;
            }

            return avatarProperties;
        }

        public async Task<GameObject> GetPreviewAvatar(string id)
        {
            byte[] data;
            try
            {
                data = await avatarAPIRequests.GetPreviewAvatar(id, avatarConfigParameters);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }

            if (ctxSource.IsCancellationRequested)
            {
                return null;
            }

            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        /// <summary>
        /// Download and import pre-created avatar.
        /// </summary>
        /// <param name="id">Avatar id</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<GameObject> GetAvatar(string id)
        {
            avatarId = id;
            byte[] data;
            try
            {
                data = await avatarAPIRequests.GetAvatar(avatarId, avatarConfigParameters);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }

            if (ctxSource.IsCancellationRequested)
            {
                return null;
            }

            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        /// <summary>
        /// Update an asset of the avatar.
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="assetType"></param>
        /// <returns>Avatar gameObject</returns>
        public async Task<GameObject> UpdateAsset(AssetType assetType, object assetId)
        {
            var payload = new AvatarProperties
            {
                Assets = new Dictionary<AssetType, object>()
            };

            payload.Assets.Add(assetType, assetId);

            byte[] data;
            try
            {
                data = await avatarAPIRequests.UpdateAvatar(avatarId, payload, avatarConfigParameters);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }

            if (ctxSource.IsCancellationRequested)
            {
                return null;
            }

            return await inCreatorAvatarLoader.Load(avatarId, bodyType, gender, data);
        }

        /// <summary>
        /// Saves the avatar from temp to permanent storage. 
        /// </summary>
        public async Task<string> Save()
        {
            try
            {
                await avatarAPIRequests.SaveAvatar(avatarId);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }

            return avatarId;
        }
        
        /// <summary>
        /// This will delete the avatar draft which have not been saved. 
        /// </summary>
        public async void DeleteDraft()
        {
            try
            {
                await avatarAPIRequests.DeleteAvatarDraft(avatarId);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// This will delete the avatar completely even from database. 
        /// </summary>
        public async Task Delete()
        {
            try
            {
                await avatarAPIRequests.DeleteAvatar(avatarId);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }
        
        public async Task<ColorPalette[]> LoadAvatarColors()
        {
            ColorPalette[] colors = null;
            try
            {
                colors = await avatarAPIRequests.GetAllAvatarColors(avatarId);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }

            return colors;
        }

        public void Dispose()
        {
            ctxSource?.Cancel();
        }
    }
}
