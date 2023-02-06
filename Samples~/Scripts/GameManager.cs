using NativeAvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace AvatarCreatorExample
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AvatarCreator avatarCreator;
        [SerializeField] private AvatarConfig inGameConfig;

        private void OnEnable()
        {
            avatarCreator.Saved += OnAvatarSaved;
        }

        private void OnDisable()
        {
            avatarCreator.Saved -= OnAvatarSaved;
        }

        private void OnAvatarSaved(string avatarId)
        {
            var startTime = Time.time;
            var avatarObjectLoader = new AvatarObjectLoader();
            avatarObjectLoader.AvatarConfig = inGameConfig;
            avatarObjectLoader.OnCompleted += (sender, args) =>
            {
                AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, args.Avatar);
                DebugPanel.AddLogWithDuration("Created avatar loaded", Time.time - startTime);
            };

            avatarObjectLoader.LoadAvatar($"{Endpoints.AVATAR_API_V1}/{avatarId}.glb");
        }
    }
}
