using System.Runtime.Serialization;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class SubscribeChannelRequest
    {
        public SubscribeChannelRequest(string videoUrl)
        {
            VideoUrl = videoUrl;
        }

        [DataMember(IsRequired = true)]
        public string VideoUrl { get; }
    }
}