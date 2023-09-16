using System;

namespace YoutubeFeeds.Core
{
    [Flags]
    public enum VideoStatus
    {
        New = 0,

        Checked = 1,

        Viewed = 2,

        Skipped = 4,
    }
}