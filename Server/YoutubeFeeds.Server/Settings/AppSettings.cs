using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server
{
    public class AppSettings : IDbSettings, IAppSettings, IYoutubeSettings
    {
        public string DbConnectionString { get; set; } = null!;
        public string UpdateSchedule { get; set; } = null!;
        public string YoutubeApiKey { get; set; } = null!;
    }
}