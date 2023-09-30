using System;
using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class UpdateStatusRequest
    {
        public UpdateStatusRequest(Guid? id, string? videoUrl, VideoStatus status)
        {
            Id = id;
            VideoUrl = videoUrl;
            Status = status;
        }

        [DataMember(IsRequired = false)]
        public Guid? Id { get; }

        [DataMember(IsRequired = false)]
        public string? VideoUrl { get; }

        [DataMember(IsRequired = true)]
        public VideoStatus Status { get; }
    }
}