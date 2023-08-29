﻿using System.Collections.Generic;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class AnalyticsRuntimeLoggerExtension
    {
        private const string RUN_AVATAR_CREATOR_SAMPLE = "Run avatar creator sample";

        public static void LogAvatarCreatorSample(this IAnalyticsRuntimeLogger _, string appId)
        {
            if (!CoreSettingsHandler.CoreSettings.EnableAnalytics) return;

#if UNITY_EDITOR
            AmplitudeEventLogger.LogEvent(RUN_AVATAR_CREATOR_SAMPLE, new Dictionary<string, object>()
            {
                { "AppId", appId }
            });
#endif

        }
    }
}
