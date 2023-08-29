using System.Linq;
using ReadyPlayerMe.AvatarCreator.Editor;
using ReadyPlayerMe.Core.Analytics;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[InitializeOnLoad]
public static class AvatarCreatorEditorAnalytics
{
    private const string AVATAR_CREATOR_MODULE_NAME = "com.readyplayerme.avatarcreator";

    static AvatarCreatorEditorAnalytics()
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
            Debug.Log(package.version);
            AnalyticsEditorLogger.EventLogger.LogAvatarCreatorImported(package.version);
        }
    }
}
