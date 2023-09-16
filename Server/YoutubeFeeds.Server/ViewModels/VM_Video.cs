using System;
using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class VM_Video
    {
        public VM_Video(Guid id, string title, string previewUrl, string videoUrl, VideoStatus status)
        {
            Id = id;
            Title = title;
            PreviewUrl = previewUrl;
            VideoUrl = videoUrl;
            Status = status;
        }

        [DataMember(IsRequired = true)]
        public Guid Id { get; }

        [DataMember(IsRequired = true)]
        public string Title { get; }

        [DataMember(IsRequired = true)]
        public string PreviewUrl { get; }

        [DataMember(IsRequired = true)]
        public string VideoUrl { get; }

        [DataMember(IsRequired = true)]
        public VideoStatus Status { get; }
    }
}