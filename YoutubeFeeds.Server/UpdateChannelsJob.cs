using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server
{
    public class UpdateChannelsJob : IJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly VideoStorage storage;

        public UpdateChannelsJob(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            storage = serviceProvider.GetRequiredService<VideoStorage>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.Write($"{DateTime.Now} Updating channels");

            var channels = await storage.GetAllChannels();

            var result = new Dictionary<Channel, Video[]>();
            foreach (var channel in channels)
            {
                Console.Write(".");
                var channelParser = new ChannelParser(channel, serviceProvider);
                var savedVideos = await channelParser.Update();
                if (savedVideos.Any())
                {
                    result.Add(channel, savedVideos);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done.");

            foreach (var (channel, savedVideos) in result)
            {
                Console.WriteLine($"Channel '{channel.Title}', {savedVideos.Length} new video(s):");
                foreach (var video in savedVideos)
                {
                    Console.WriteLine($"- {video.Title}");
                }
            }
        }
    }
}