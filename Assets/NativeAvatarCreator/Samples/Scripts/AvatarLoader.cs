using System.Threading.Tasks;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AvatarLoader : MonoBehaviour
    {
        [SerializeField] private AvatarConfig avatarConfig;
        [SerializeField] private DataStore dataStore;

        public async Task<GameObject> LoadAvatar(string avatarId, byte[] data)
        {
            var avatarMetadata = new AvatarMetadata();
            avatarMetadata.BodyType = dataStore.AvatarProperties.BodyType == "fullbody" ? BodyType.FullBody : BodyType.HalfBody;
            avatarMetadata.OutfitGender = dataStore.AvatarProperties.Gender switch
            {
                "male" => OutfitGender.Masculine,
                "female" => OutfitGender.Feminine,
                _ => OutfitGender.Neutral
            };
            // TODO set last modified

            var context = new AvatarContext();
            context.Bytes = data;
            context.AvatarUri.Guid = avatarId;
            context.AvatarCachingEnabled = false;
            context.Metadata = avatarMetadata;
            context.AvatarConfig = avatarConfig;
            context.ParametersHash = AvatarCache.GetAvatarConfigurationHash(avatarConfig);

            var executor = new OperationExecutor<AvatarContext>(new IOperation<AvatarContext>[]
            {
                new GltFastAvatarImporter(),
                new AvatarProcessor()
            });
            // executor.ProgressChanged += ProgressChanged;
            // executor.Timeout = Timeout;

            try
            {
                context = await executor.Execute(context);
            }
            catch (CustomException exception)
            {
                throw new CustomException(executor.IsCancelled ? FailureType.OperationCancelled : exception.FailureType, exception.Message);
            }

            var avatar = (GameObject) context.Data;
            avatar.SetActive(true);
            avatar.AddComponent<RotateAvatar>();
            return avatar;
        }
    }
}
