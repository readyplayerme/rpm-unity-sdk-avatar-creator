﻿using System.Collections.Generic;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.AvatarCreator.Editor
{
    public static class AnalyticsEditorLoggerExtension
    {
        private const string AVATAR_CREATOR_IMPORTED = "avatar creator imported";
        private const string AVATAR_CREATOR_VERSION = "avatar creator version";

        public static void LogAvatarCreatorImported(this IAnalyticsEditorLogger _, string packageVersion)
        {
            if (!AnalyticsEditorLogger.IsEnabled) return;
            AmplitudeEventLogger.LogEvent(AVATAR_CREATOR_IMPORTED, new Dictionary<string, object>
            {
                { AVATAR_CREATOR_VERSION, packageVersion }
            });
        }
    }
}
