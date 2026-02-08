using System;
using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class VM_UpdateStatistics
    {
        public VM_UpdateStatistics(UpdateStatistics statistics)
        {
            var minutesSinceLastCheck = (int)(DateTime.UtcNow - statistics.LastCheck).TotalMinutes;
            LastCheck = minutesSinceLastCheck;
            LastCheckDuration = statistics.LastCheckDuration;
            SuccessCount = statistics.SuccessCount;
            FailCount = statistics.FailCount;

            LastCheckState = minutesSinceLastCheck switch
            {
                < 40 => OpState.Green,
                < 60 => OpState.Yellow,
                _ => OpState.Red
            };

            LastCheckDurationState = LastCheckDuration switch
            {
                < 1000 => OpState.Green,
                < 2000 => OpState.Yellow,
                _ => OpState.Red
            };
        }

        [DataMember(IsRequired = true)]
        public int LastCheck { get; }

        [DataMember(IsRequired = true)]
        public OpState LastCheckState { get; }

        [DataMember(IsRequired = true)]
        public int LastCheckDuration { get; }

        [DataMember(IsRequired = true)]
        public OpState LastCheckDurationState { get; }

        [DataMember(IsRequired = true)]
        public int SuccessCount { get; }

        [DataMember(IsRequired = true)]
        public int FailCount { get; }
    }
}