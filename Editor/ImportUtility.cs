using ReadyPlayerMe.Core.Analytics;
using UnityEditor;

namespace ReadyPlayerMe.AvatarCreator.Editor
{
    [InitializeOnLoad]
    public class ImportUtility
    {
        private const string AVATAR_CREATOR_VERSION = "v1.0.1";

        static ImportUtility()
        {
            EditorApplication.delayCall += OnRegisteredPackages;
        }

        ~ImportUtility()
        {
            EditorApplication.delayCall += OnRegisteredPackages;
        }

        private static void OnRegisteredPackages()
        {
            AnalyticsEditorLogger.EventLogger.LogAvatarCreatorImported(AVATAR_CREATOR_VERSION);
        }
    }
}
