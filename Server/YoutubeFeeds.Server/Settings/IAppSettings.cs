namespace YoutubeFeeds.Server
{
    internal interface IAppSettings
    {
        string UpdateSchedule { get; }
        string? RssProxy { get; }
    }
}
