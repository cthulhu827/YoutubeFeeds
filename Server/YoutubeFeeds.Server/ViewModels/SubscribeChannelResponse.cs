using System.Runtime.Serialization;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class SubscribeChannelResponse
    {
        public SubscribeChannelResponse(bool success)
        {
            Success = success;
        }

        [DataMember(IsRequired = true)]
        public bool Success { get; }
    }
}