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
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://www.youtube.com/feeds/videos.xml?channel_id={channel.YoutubeId}");
            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
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