using System;

namespace YoutubeFeeds.Core
{
    public class Channel
    {
        public Channel(Guid id, string youtubeId, string title, DateTime? lastUpdate, DateTime? lastCheck, int? lastCheckDuration, bool? lastCheckSuccess)
        {
            Id = id;
            YoutubeId = youtubeId;
            Title = title;
            LastUpdate = lastUpdate;
            LastCheck = lastCheck;
            LastCheckDuration = lastCheckDuration;
            LastCheckSuccess = lastCheckSuccess;
        }

        public Guid Id { get; }
        public string YoutubeId { get; }
        public string Title { get; }
        public DateTime? LastUpdate { get; }
        public DateTime? LastCheck { get; }
        public int? LastCheckDuration { get; }
        public bool? LastCheckSuccess { get; }

        public string RssUrl => $"https://www.youtube.com/feeds/videos.xml?channel_id={YoutubeId}";

        #region Db details

        public const string TableName = "channels";
        public const string YoutubeIdConstraint = "channels_youtube_id_uniq";

        public const string IdCol = "id";
        public const string YoutubeIdCol = "youtube_id";
        public const string TitleCol = "title";
        public const string LastUpdateCol = "last_update";
        public const string LastCheckCol = "last_check";
        public const string LastCheckDurationCol = "last_check_duration";
        public const string LastCheckSuccessCol = "last_check_success";
        public const string IdxCol = "idx";

        public static readonly string AllFieldsWithAliases = string.Join(", ",
            $"{IdCol} \"{nameof(Id)}\"",
            $"{YoutubeIdCol} \"{nameof(YoutubeId)}\"",
            $"{TitleCol} \"{nameof(Title)}\"",
            $"{LastUpdateCol} \"{nameof(LastUpdate)}\"",
            $"{LastCheckCol} \"{nameof(LastCheck)}\"",
            $"{LastCheckDurationCol} \"{nameof(LastCheckDuration)}\"",
            $"{LastCheckSuccessCol} \"{nameof(LastCheckSuccess)}\""
        );

        #endregion
    }
}