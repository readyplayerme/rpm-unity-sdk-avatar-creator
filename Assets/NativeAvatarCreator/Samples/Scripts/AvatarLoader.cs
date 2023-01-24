using System.Threading.Tasks;
using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class AvatarLoader : MonoBehaviour
    {
        [SerializeField] private DataStore dataStore;

        public async Task<GameObject> LoadAvatar(string avatarId, byte[] data)
        {
            var avatarMetadata = new AvatarMetadata();
            avatarMetadata.BodyType = dataStore.AvatarProperties.BodyType == AvatarPropertiesConstants.FULL_BODY ? BodyType.FullBody : BodyType.HalfBody;
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

            var executor = new OperationExecutor<AvatarContext>(new IOperation<AvatarContext>[]
            {
                new GltFastAvatarImporter(),
                new AvatarProcessor()
            });

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
