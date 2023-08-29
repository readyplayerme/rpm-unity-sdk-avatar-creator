using System.Collections.Generic;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.AvatarCreator.Editor
{
    public static class AnalyticsEditorLoggerExtension
    {
        private const string AVATAR_CREATOR_IMPORTED = "Avatar Creator Imported";

        public static void LogAvatarCreatorImported(this IAnalyticsEditorLogger _, string packageVersion)
        {
            if (!AnalyticsEditorLogger.IsEnabled) return;
            AmplitudeEventLogger.LogEvent(AVATAR_CREATOR_IMPORTED, new Dictionary<string, object>
            {
                { "Avatar Creator Version", packageVersion }
            });
        }
    }
}
