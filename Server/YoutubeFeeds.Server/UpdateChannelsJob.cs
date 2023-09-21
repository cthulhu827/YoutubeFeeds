using Quartz;
using System.Threading.Tasks;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server
{
    public class UpdateChannelsJob : IJob
    {
        private readonly ChannelService channelService;

        public UpdateChannelsJob(ChannelService channelService)
        {
            this.channelService = channelService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await channelService.UpdateAllChannels();
        }
    }
}