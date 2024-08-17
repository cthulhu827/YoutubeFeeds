using System;
using System.Collections.Generic;
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

        public ChannelParser(Channel channel, IServiceProvider serviceProvider)
        {
            this.channel = channel;
            storage = serviceProvider.GetRequiredService<VideoStorage>();
        }

        public async Task<Video[]> Update()
        {
            // Получаем RSS
            var rssXml = await GetRss();
            if (string.IsNullOrWhiteSpace(rssXml))
            {
                return Array.Empty<Video>();
            }

            // Парасим RSS, получаем все содержащиеся в нём видео
            var videosFromRss = ParseChannelRss(rssXml);

            // Находим видео, которых ещё нет в базе
            var newIds = await GetNewIds(videosFromRss);

            // и сохраняем их
            var videosToSave = videosFromRss.Where(v => newIds.Contains(v.YoutubeId)).ToArray();
            await storage.InsertVideos(videosToSave);

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
                Console.WriteLine($"{DateTime.Now} Requesting RSS for channel '{channel.Title}': {channel.RssUrl}");
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var request = new HttpRequestMessage(HttpMethod.Get, channel.RssUrl);
                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{DateTime.Now} Got RSS for channel '{channel.Title}': {result.Length} bytes");
                return result;
            }
            catch (Exception)
            {
                Console.WriteLine($"{DateTime.Now} Failed to get RSS for channel '{channel.Title}'");
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
                var videoUrl = node.SelectSingleNode("ns:link/@href", nsmgr)!.InnerText;
                var queryString = new Uri(videoUrl).Query;
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
                var videoId = queryDictionary["v"];

                var title = node.SelectSingleNode("ns:title", nsmgr)!.InnerText;

                var published = node.SelectSingleNode("ns:published", nsmgr)!.InnerText;
                var publishedDateTime = DateTime.Parse(published);

                var video = new Video(Guid.NewGuid(), videoId, title, VideoStatus.New, publishedDateTime, channel.Id);
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