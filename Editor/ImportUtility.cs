using System.Linq;
using ReadyPlayerMe.Core.Analytics;
using UnityEditor;
using UnityEditor.PackageManager;

namespace ReadyPlayerMe.AvatarCreator.Editor
{
    [InitializeOnLoad]
    public static class ImportUtility
    {
        private const string AVATAR_CREATOR_MODULE_NAME = "com.readyplayerme.avatarcreator";

        static ImportUtility()
        {
            Events.registeredPackages += OnRegisteredPackages;
        }

        private static void OnRegisteredPackages(PackageRegistrationEventArgs args)
        {
            Events.registeredPackages -= OnRegisteredPackages;
#if RPM_DEVELOPMENT
            return;
#endif
            var package = args.added.FirstOrDefault(p => p.name == AVATAR_CREATOR_MODULE_NAME);

            if (args.added != null && package != null)
            {
                AnalyticsEditorLogger.EventLogger.LogAvatarCreatorImported(package.version);
            }
        }
    }
}
