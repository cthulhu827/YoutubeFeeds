using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeFeeds.Core
{
    public class VideoService
    {
        private readonly VideoStorage storage;

        public VideoService(VideoStorage storage)
        {
            this.storage = storage;
        }

        
    }
}