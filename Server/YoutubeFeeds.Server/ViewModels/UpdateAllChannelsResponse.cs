using System.Runtime.Serialization;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class UpdateAllChannelsResponse
    {
        public UpdateAllChannelsResponse(int newVideosCount)
        {
            NewVideosCount = newVideosCount;
        }

        [DataMember(IsRequired = true)]
        public int NewVideosCount { get; }
    }
}
