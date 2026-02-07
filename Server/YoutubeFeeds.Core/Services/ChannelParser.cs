using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace YoutubeFeeds.Core
{
    public class ChannelParser
    {
        private readonly Channel channel;
        private readonly VideoStorage storage;
        private readonly string? rssProxy;

        public ChannelParser(Channel channel, IServiceProvider serviceProvider)
        {
            this.channel = channel;
            storage = serviceProvider.GetRequiredService<VideoStorage>();
            var appSettings = serviceProvider.GetService<IRssProxySettings>();
            rssProxy = appSettings?.RssProxy;
        }

        public async Task<ChannelUpdate> Update(DateTime now)
        {
            var stopwatch = Stopwatch.StartNew();
            var videos = await DoUpdate();
            stopwatch.Stop();

            var lastCheckDuration = (int)stopwatch.ElapsedMilliseconds;
            var lastCheckSuccess = videos != null;
            var lastUpdate = videos != null && videos.Length > 0 ? now : (DateTime?)null;
            var lastCheck = now;

            return new ChannelUpdate(channel.Id, lastUpdate, lastCheck, lastCheckDuration, lastCheckSuccess)
            {
                Videos = videos ?? Array.Empty<Video>()
            };
        }

        private async Task<Video[]?> DoUpdate()
        {
            // Получаем RSS
            var rssXml = await GetRss();
            if (string.IsNullOrWhiteSpace(rssXml))
            {
                return null;
            }

            IReadOnlyCollection<Video> videosFromRss;
            try
            {
                // Парасим RSS, получаем все содержащиеся в нём видео
                videosFromRss = ParseChannelRss(rssXml);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} Failed to parse RSS for channel '{channel.Title}': {e}");
                return null;
            }

            // Находим видео, которых ещё нет в базе
            var newIds = await GetNewIds(videosFromRss);

            // и сохраняем их
            var videosToSave = videosFromRss.Where(v => newIds.Contains(v.YoutubeId)).ToArray();
            if (videosToSave.Any()) await storage.InsertVideos(videosToSave);

            return videosToSave;
        }

        private async Task<string> GetRss()
        {
            const int tryCount = 3;
            var result = string.Empty;
            for (int i = 0; i < tryCount; i++)
            {
                result = await TryGetRss();
                if (!string.IsNullOrWhiteSpace(result)) break;
            }

            return result;
        }

        private async Task<string> TryGetRss()
        {
            try
            {
                var requestUrl = string.IsNullOrWhiteSpace(rssProxy)
                    ? channel.RssUrl
                    : $"{rssProxy}/api/get-rss?url={Uri.EscapeDataString(channel.RssUrl)}";

                Console.WriteLine($"{DateTime.Now} Requesting RSS for channel '{channel.Title}': {requestUrl}");
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{DateTime.Now} Got RSS for channel '{channel.Title}': {result.Length} bytes");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} Failed to get RSS for channel '{channel.Title}': {e}");
                return string.Empty;
            }
        }

        private IReadOnlyCollection<Video> ParseChannelRss(string rssXml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rssXml);

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns", "http://www.w3.org/2005/Atom");

            var nodes = xmlDoc.DocumentElement?.SelectNodes("ns:entry", nsmgr);
            var result = new List<Video>();
            foreach (XmlNode node in nodes!)
            {
                string videoId;
                var videoUrl = node.SelectSingleNode("ns:link/@href", nsmgr)!.InnerText;
                bool isShort = videoUrl.Contains("/shorts/");
                if (isShort)
                    videoId = new Uri(videoUrl).Segments.Last();
                else
                {
                    var queryString = new Uri(videoUrl).Query;
                    var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
                    videoId = queryDictionary["v"];
                }

                var title = node.SelectSingleNode("ns:title", nsmgr)!.InnerText;

                var published = node.SelectSingleNode("ns:published", nsmgr)!.InnerText;
                var publishedDateTime = DateTime.Parse(published);

                var video = new Video(Guid.NewGuid(), videoId, title, VideoStatus.New,
                    publishedDateTime, channel.Id, isShort);
                result.Add(video);
            }

            return result;
        }

        private async Task<string[]> GetNewIds(IEnumerable<Video> videosFromRss)
        {
            var videoIdsFromRss = videosFromRss.Select(video => video.YoutubeId).ToArray();
            return await storage.CheckIds(videoIdsFromRss.ToArray());
        }
    }
}