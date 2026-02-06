using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace YoutubeFeeds.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .Build()
                .Run();
        }

        private static IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.local.json", true);

            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrWhiteSpace(envName))
                configurationBuilder.AddJsonFile($"appsettings.{envName}.json", false);

            return configurationBuilder
                .AddEnvironmentVariables()
                .Build();
        }
    }
}