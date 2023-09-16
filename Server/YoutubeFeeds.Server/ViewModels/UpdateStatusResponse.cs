using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class UpdateStatusResponse
    {
        public UpdateStatusResponse(VideoStatus newStatus)
        {
            NewStatus = newStatus;
        }

        [DataMember(IsRequired = true)]
        public VideoStatus NewStatus { get; }
    }
}