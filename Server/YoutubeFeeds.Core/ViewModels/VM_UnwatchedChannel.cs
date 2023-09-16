using System;
using System.Runtime.Serialization;

namespace YoutubeFeeds.Core
{
    [DataContract]
    public class VM_UnwatchedChannel
    {
        public VM_UnwatchedChannel(Guid id, string title, int _new, int _checked)
        {
            Id = id;
            Title = title;
            New = _new;
            Checked = _checked;
        }

        [DataMember(IsRequired = true)]
        public Guid Id { get; }

        [DataMember(IsRequired = true)]
        public string Title { get; }

        [DataMember(IsRequired = true)]
        public int New { get; }

        [DataMember(IsRequired = true)]
        public int Checked { get; }
    }
}