using System;

namespace YoutubeFeeds.Core
{
    public static class Helper
    {
        public static string? GetVideoId(this string videoUrl)
        {
            var queryString = new Uri(videoUrl).Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
            return queryDictionary["v"];
        }
    }
}