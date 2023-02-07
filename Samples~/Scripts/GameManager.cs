using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.AvatarLoader;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AvatarCreatorManager avatarCreatorManager;
        [SerializeField] private AvatarConfig inGameConfig;

        private void OnEnable()
        {
            avatarCreatorManager.Saved += OnAvatarSaved;
        }

        private void OnDisable()
        {
            avatarCreatorManager.Saved -= OnAvatarSaved;
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
