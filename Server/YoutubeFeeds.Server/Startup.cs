using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using YoutubeFeeds.Core;

namespace YoutubeFeeds.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));

            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            services.AddSingleton<IDbSettings>(appSettings);
            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IYoutubeSettings>(appSettings);

            services.AddControllers();

            AddQuartz(services, appSettings);

            services.AddSingleton<DbConnectionFactory>();
            services.AddSingleton<VideoStorage>();
            services.AddSingleton<ChannelService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("AllowAnyOrigin");
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void AddQuartz(IServiceCollection services, IAppSettings appSettings)
        {
            services.AddQuartz(q =>
            {
                //q.UseMicrosoftDependencyInjectionScopedJobFactory();
                var jobKey = new JobKey("UpdateChannelsJob");
                q.AddJob<UpdateChannelsJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("UpdateChannelsJob-trigger")
                    .WithCronSchedule(appSettings.UpdateSchedule)
                );
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}