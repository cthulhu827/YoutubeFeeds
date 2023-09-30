using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YoutubeFeeds.Core;
using YoutubeFeeds.Server.ViewModels;

namespace YoutubeFeeds.Server.Controllers
{
    public class VideosController : ControllerBase
    {
        private readonly VideoStorage storage;
        private readonly ChannelService channelService;

        public VideosController(VideoStorage storage, ChannelService channelService)
        {
            this.storage = storage;
            this.channelService = channelService;
        }

        [HttpGet]
        [Route("/api/videos/all/{channelId?}")]
        public async Task<VM_Video[]> GetAllVideos(string? channelId)
        {
            Console.WriteLine("ChannelId: " + channelId);
            var id = string.IsNullOrWhiteSpace(channelId) ? (Guid?)null : Guid.Parse(channelId);
            var videos = await storage.GetUnwatchedVideos(id);
            return videos
                .Select(v => new VM_Video(v.Id, v.Title, v.PreviewUrl(), v.VideoUrl(), v.Status))
                .ToArray();
        }

        [HttpGet]
        [Route("/api/channels/unwatched")]
        public async Task<VM_UnwatchedChannel[]> GetUnwatchedChannels()
        {
            var result = await channelService.GetUnwatchedChannels();
            return result.ToArray();
        }

        [HttpPost]
        [Route("/api/videos/update_status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request.Id != null)
            {
                Console.WriteLine($"Updating {request.Id.Value}");
                await storage.UpdateVideoStatus(request.Id.Value, request.Status);
                return Ok(new UpdateStatusResponse(request.Status));
            }

            if (!string.IsNullOrWhiteSpace(request.VideoUrl))
            {
                var youtubeId = request.VideoUrl.GetVideoId();
                if (!string.IsNullOrWhiteSpace(youtubeId))
                {
                    Console.WriteLine($"Updating {youtubeId}");
                    await storage.UpdateVideoStatus(youtubeId, request.Status);
                    return Ok(new UpdateStatusResponse(request.Status));
                }
            }

            return BadRequest($"{nameof(request.Id)} or ${nameof(request.VideoUrl)} must be defined");
        }

        [HttpPost]
        [Route("/api/videos/subscribe_channel")]
        public async Task<SubscribeChannelResponse> SubscribeChannel([FromBody] SubscribeChannelRequest request)
        {
            var subscribed = await channelService.SubscribeChannel(request.VideoUrl);
            return new SubscribeChannelResponse(subscribed);
        }

        [HttpPost]
        [Route("/api/videos/update_all_channels")]
        public async Task<UpdateAllChannelsResponse> UpdateAllChannels()
        {
            var result = await channelService.UpdateAllChannels();
            return new UpdateAllChannelsResponse(result);
        }
    }
}