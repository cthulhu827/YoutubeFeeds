using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
        }

        private static void Log(string text)
        {
            System.Console.WriteLine($"{DateTime.Now} {text}");
        }
    }
}