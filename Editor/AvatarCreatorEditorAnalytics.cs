using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[InitializeOnLoad]
public static class AvatarCreatorEditorAnalytics
{
    private const string AVATAR_CREATOR_MODULE_NAME = "com.readyplayerme.avatarcreator";

    static AvatarCreatorEditorAnalytics()
    {
        Debug.Log("Avatar Creator AvatarCreatorEditorAnalytics");
        
        Events.registeredPackages += OnRegisteredPackages;
    }

    private static void OnRegisteredPackages(PackageRegistrationEventArgs args)
    {
        Events.registeredPackages -= OnRegisteredPackages;
#if RPM_DEVELOPMENT
            return;
#endif
        Debug.Log("Avatar Creator Call");
        
        if (args.added != null && args.added.Any(p => p.name == AVATAR_CREATOR_MODULE_NAME))
        {
            Debug.Log("Avatar Creator module installed");
        }
    }
}
