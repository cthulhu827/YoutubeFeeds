using System.Runtime.Serialization;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server.ViewModels
{
    [DataContract]
    public class VM_Channels
    {
        public VM_Channels(VM_UnwatchedChannel[] channels, VM_UpdateStatistics? statistics)
        {
            Channels = channels;
            Statistics = statistics;
        }

        [DataMember(IsRequired = true)]
        public VM_UnwatchedChannel[] Channels { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public VM_UpdateStatistics? Statistics { get; }
    }
}
