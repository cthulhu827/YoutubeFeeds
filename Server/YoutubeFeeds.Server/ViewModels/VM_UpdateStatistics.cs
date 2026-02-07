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
            LastCheck = $"{minutesSinceLastCheck} min";
            LastCheckDuration = statistics.LastCheckDuration;
            SuccessCount = statistics.SuccessCount;
            FailCount = statistics.FailCount;
        }

        [DataMember(IsRequired = true)]
        public string LastCheck { get; }

        [DataMember(IsRequired = true)]
        public int LastCheckDuration { get; }

        [DataMember(IsRequired = true)]
        public int SuccessCount { get; }

        [DataMember(IsRequired = true)]
        public int FailCount { get; }
    }
}
