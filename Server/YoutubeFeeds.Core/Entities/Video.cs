using System;

namespace YoutubeFeeds.Core
{
    public class Video
    {
        public Video(Guid id, string youtubeId, string title, VideoStatus status, DateTime published,
            Guid? channelId, bool? isShort)
        {
            Id = id;
            YoutubeId = youtubeId;
            Title = title;
            Status = status;
            Published = published;
            ChannelId = channelId;
            IsShort = isShort;
        }

        public Guid Id { get; }
        public string YoutubeId { get; }
        public string Title { get; }
        public VideoStatus Status { get; }
        public DateTime Published { get; }
        public Guid? ChannelId { get; }
        public bool? IsShort { get; }

        public string VideoUrl() => $"https://www.youtube.com/watch?v={YoutubeId}";

        public string PreviewUrl() => $"https://img.youtube.com/vi/{YoutubeId}/0.jpg";

        #region Db details

        public const string TableName = "videos";

        public const string IdCol = "id";
        public const string YoutubeIdCol = "youtube_id";
        public const string TitleCol = "title";
        public const string StatusCol = "status";
        public const string PublishedCol = "published";
        public const string ChannelIdCol = "channel_id";
        public const string IsShortCol = "is_short";

        public static readonly string AllFieldsWithAliases = string.Join(", ",
            $"{IdCol} \"{nameof(Id)}\"",
            $"{YoutubeIdCol} \"{nameof(YoutubeId)}\"",
            $"{TitleCol} \"{nameof(Title)}\"",
            $"{StatusCol} \"{nameof(Status)}\"",
            $"{PublishedCol} \"{nameof(Published)}\"",
            $"{ChannelIdCol} \"{nameof(ChannelId)}\"",
            $"{IsShortCol} \"{nameof(IsShort)}\""
        );

        #endregion
    }
}