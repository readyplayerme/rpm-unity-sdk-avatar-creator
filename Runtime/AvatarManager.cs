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
        private const string TAG = nameof(AvatarManager);
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
        /// Create a new avatar.
        /// </summary>
        /// <param name="avatarProperties">Properties which describes avatar</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<GameObject> CreateAvatar(AvatarProperties avatarProperties)
        {
            GameObject avatar = null;
            try
            {
                avatarProperties = await avatarAPIRequests.CreateNewAvatar(avatarProperties);
                if (ctxSource.IsCancellationRequested)
                {
                    return null;
                }

                avatarId = avatarProperties.Id;
                avatar = await GetAvatar(avatarId, true);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return avatar;
            }

            return avatar;
        }

        /// <summary>
        /// Create a new avatar from a provided template.
        /// </summary>
        /// <param name="id">Template id</param>
        /// <param name="partner">Partner name</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<(GameObject, AvatarProperties)> CreateAvatarFromTemplate(string id, string partner)
        {
            GameObject avatar = null;
            var avatarProperties = new AvatarProperties();
            try
            {
                avatarProperties = await avatarAPIRequests.CreateFromTemplateAvatar(
                    id,
                    partner,
                    bodyType
                );
                if (ctxSource.IsCancellationRequested)
                {
                    return (null, avatarProperties);
                }

                avatarId = avatarProperties.Id;
                avatar = await GetAvatar(avatarId, true);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return (avatar, avatarProperties);
            }

            return (avatar, avatarProperties);
        }
       
        /// <summary>
        /// Precompile an avatar on server to increase the fetching speed.
        /// </summary>
        /// <param name="id">Avatar id</param>
        /// <param name="precompileData">Precompiled data for assets</param>
        public async void PrecompileAvatar(string id, PrecompileData precompileData)
        {
            try
            {
                await avatarAPIRequests.PrecompileAvatar(id, precompileData, avatarConfigParameters);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                SDKLogger.LogWarning(TAG, "Precompiled avatar request failed.");
            }

            if (ctxSource.IsCancellationRequested)
            {
                SDKLogger.LogWarning(TAG, "Precompiled avatar request cancelled.");
            }
        }

        /// <summary>
        /// Download and import avatar.
        /// </summary>
        /// <param name="id">Avatar id</param>
        /// <param name="isPreview">Whether its a preview avatar</param>
        /// <returns>Avatar gameObject</returns>
        public async Task<GameObject> GetAvatar(string id, bool isPreview = false)
        {
            avatarId = id;
            byte[] data;
            try
            {
                data = await avatarAPIRequests.GetAvatar(avatarId, isPreview, avatarConfigParameters);
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
        /// <param name="category"></param>
        /// <returns>Avatar gameObject</returns>
        public async Task<GameObject> UpdateAsset(Category category, object assetId)
        {
            var payload = new AvatarProperties
            {
                Assets = new Dictionary<Category, object>()
            };

            if (category == Category.Top || category == Category.Bottom || category == Category.Footwear)
            {
                payload.Assets.Add(Category.Outfit, string.Empty);
            }

            payload.Assets.Add(category, assetId);

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
        public async void Delete(bool isDraft)
        {
            try
            {
                await avatarAPIRequests.DeleteAvatar(avatarId, isDraft);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Dispose()
        {
            ctxSource?.Cancel();
        }
    }
}
