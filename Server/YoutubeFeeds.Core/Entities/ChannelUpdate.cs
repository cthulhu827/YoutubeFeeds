using System;

namespace YoutubeFeeds.Core
{
    public class ChannelUpdate
    {
        public ChannelUpdate(Guid id, DateTime? lastUpdate, DateTime? lastCheck, int? lastCheckDuration, bool? lastCheckSuccess)
        {
            Id = id;
            LastUpdate = lastUpdate;
            LastCheck = lastCheck;
            LastCheckDuration = lastCheckDuration;
            LastCheckSuccess = lastCheckSuccess;
        }

        public Guid Id { get; }
        public DateTime? LastUpdate { get; }
        public DateTime? LastCheck { get; }
        public int? LastCheckDuration { get; }
        public bool? LastCheckSuccess { get; }

        public Video[] Videos { get; set; } = Array.Empty<Video>();
    }
}
