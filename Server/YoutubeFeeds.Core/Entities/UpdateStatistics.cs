using System;

namespace YoutubeFeeds.Core
{
    public class UpdateStatistics
    {
        public UpdateStatistics(DateTime lastCheck, int lastCheckDuration, int successCount, int failCount)
        {
            LastCheck = lastCheck;
            LastCheckDuration = lastCheckDuration;
            SuccessCount = successCount;
            FailCount = failCount;
        }

        public DateTime LastCheck { get; }
        public int LastCheckDuration { get; }
        public int SuccessCount { get; }
        public int FailCount { get; }
    }
}