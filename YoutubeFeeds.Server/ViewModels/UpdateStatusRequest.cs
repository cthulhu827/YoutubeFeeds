using System;
using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class UpdateStatusRequest
    {
        public UpdateStatusRequest(Guid id, VideoStatus status)
        {
            Id = id;
            Status = status;
        }

        [DataMember(IsRequired = true)]
        public Guid Id { get; }

        [DataMember(IsRequired = true)]
        public VideoStatus Status { get; }
    }
}