using System;

namespace YoutubeFeeds.Core
{
    public class Channel
    {
        public Channel(Guid id, string youtubeId, string title)
        {
            Id = id;
            YoutubeId = youtubeId;
            Title = title;
        }

        public Guid Id { get; }
        public string YoutubeId { get; }
        public string Title { get; }

        public string RssUrl => $"https://www.youtube.com/feeds/videos.xml?channel_id={YoutubeId}";

        #region Db details

        public const string TableName = "channels";
        public const string YoutubeIdConstraint = "channels_youtube_id_uniq";

        public const string IdCol = "id";
        public const string YoutubeIdCol = "youtube_id";
        public const string TitleCol = "title";

        public static readonly string AllFieldsWithAliases = string.Join(", ",
            $"{IdCol} \"{nameof(Id)}\"",
            $"{YoutubeIdCol} \"{nameof(YoutubeId)}\"",
            $"{TitleCol} \"{nameof(Title)}\""
        );

        #endregion
    }
}