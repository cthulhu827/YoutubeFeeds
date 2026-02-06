using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server
{
    public class AppSettings : IDbSettings, IAppSettings, IYoutubeSettings, IRssProxySettings
    {
        public string DbConnectionString { get; set; } = null!;
        public string UpdateSchedule { get; set; } = null!;
        public string YoutubeApiKey { get; set; } = null!;
        public string? RssProxy { get; set; }
    }
}