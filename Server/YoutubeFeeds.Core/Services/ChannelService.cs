using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YoutubeFeeds.Core
{
    public class ChannelService
    {
        private readonly VideoStorage storage;
        private readonly IYoutubeSettings youtubeSettings;

        public ChannelService(VideoStorage storage, IYoutubeSettings youtubeSettings)
        {
            this.storage = storage;
            this.youtubeSettings = youtubeSettings;
        }

        public async Task<bool> SubscribeChannel(string videoUrl)
        {
            Console.WriteLine($"Video URL: {videoUrl}");
            var videoId = videoUrl.GetVideoId();
            if (string.IsNullOrWhiteSpace(videoId))
            {
                Console.WriteLine("Video Id not found.");
                return false;
            }

            Console.WriteLine($"Video Id: {videoId}");
            var (channelId, channelTitle) = await GetChannel(videoId);
            Console.WriteLine($"Got channel info: Id = {channelId}, Title = {channelTitle}");
            var channel = new Channel(Guid.NewGuid(), channelId, channelTitle);
            await storage.SaveChannel(channel);
            // get channel id by video id https://www.googleapis.com/youtube/v3/videos?part=snippet&id=csJPwynhWkA&key=AIzaSyBoyPu4xHak1m76G3Db-mT21m9hP-KJrm4
            // get channel info by channel id https://www.googleapis.com/youtube/v3/channels?part=snippet&id=UC_Q1vhf7wcR_zGlc5ahAg0A&key=AIzaSyBoyPu4xHak1m76G3Db-mT21m9hP-KJrm4

            return true;
        }

        public async Task<IEnumerable<VM_UnwatchedChannel>> GetUnwatchedChannels()
        {
            var videos = await storage.GetUnwatchedVideos(null);
            var channelIds = videos
                .Select(video => video.ChannelId)
                .Where(channelId => channelId != null)
                .Cast<Guid>()
                .Distinct()
                .ToArray();
            var channels = await storage.GetChannels(channelIds);
            var result = new List<VM_UnwatchedChannel>();
            foreach (var channel in channels)
            {
                var newCount = videos.Count(v => v.Status == VideoStatus.New && v.ChannelId == channel.Id);
                var checkedCount = videos.Count(v => v.Status == VideoStatus.Checked && v.ChannelId == channel.Id);
                var vm = new VM_UnwatchedChannel(channel.Id, channel.Title, newCount, checkedCount);
                result.Add(vm);
            }

            return result;
        }

        private async Task<(string, string)> GetChannel(string videoId)
        {
            var client = new HttpClient();
            var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={videoId}&key={youtubeSettings.YoutubeApiKey}";
            Console.WriteLine(url);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            var data = (JObject)JsonConvert.DeserializeObject(json);
            var channelId = data!.SelectToken("items[0].snippet.channelId")!.Value<string>();
            var channelTitle = data!.SelectToken("items[0].snippet.channelTitle")!.Value<string>();

            return (channelId, channelTitle);
        }
    }
}