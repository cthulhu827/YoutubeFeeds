using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = await File.ReadAllTextAsync(@"d:\w\rss.xml");
            var videos = ParseChannelRss(text);
            System.Console.WriteLine($"### {videos.Count}");
        }

        private static void Log(string text)
        {
            System.Console.WriteLine($"{DateTime.Now} {text}");
        }
        
        private static IReadOnlyCollection<Video> ParseChannelRss(string rssXml)
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

                var video = new Video(Guid.NewGuid(), videoId, title, VideoStatus.New, publishedDateTime, Guid.NewGuid(), isShort);
                System.Console.WriteLine($"### {isShort} {videoId} -> {video.Title}");
                result.Add(video);
            }

            return result;
        }
    }
}