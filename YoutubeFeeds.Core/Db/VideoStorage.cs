using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace YoutubeFeeds.Core
{
    /// <summary>
    /// Класс для чтения данных из БД.
    /// </summary>
    public class VideoStorage
    {
        private readonly DbConnectionFactory dbConnectionFactory;

        public VideoStorage(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Video[]> GetUnwatchedVideos(Guid? channelId)
        {
            var statuses = new[] { (int)VideoStatus.New, (int)VideoStatus.Checked };
            var channelIdCondition = channelId == null
                ? string.Empty
                : $"and {Video.ChannelIdCol} = @{nameof(channelId)} ";
            var query =
                $"select {Video.AllFieldsWithAliases} " +
                $"from {Video.TableName} " +
                $"where {Video.StatusCol} = any(@{nameof(statuses)}) " +
                channelIdCondition +
                $"order by {Video.PublishedCol} desc ";
            using (var context = await dbConnectionFactory.Open())
            {
                var result = await context.QueryAsync<Video>(query, new { statuses, channelId });
                return result.ToArray();
            }
        }

        public async Task<Channel[]> GetAllChannels()
        {
            var query =
                $"select {Channel.AllFieldsWithAliases} " +
                $"from {Channel.TableName} ";
            using (var context = await dbConnectionFactory.Open())
            {
                var result = await context.QueryAsync<Channel>(query);
                return result.ToArray();
            }
        }

        public async Task<Channel[]> GetChannels(Guid[] channelIds)
        {
            var query =
                $"select {Channel.AllFieldsWithAliases} " +
                $"from {Channel.TableName} " +
                $"where {Channel.IdCol} = any(@{nameof(channelIds)})";
            using (var context = await dbConnectionFactory.Open())
            {
                var result = await context.QueryAsync<Channel>(query, new { channelIds });
                return result.ToArray();
            }
        }

        public async Task InsertVideos(Video[] videos)
        {
            var query =
                $"insert into {Video.TableName} (" +
                $"{Video.IdCol}, " +
                $"{Video.YoutubeIdCol}, " +
                $"{Video.TitleCol}, " +
                $"{Video.StatusCol}, " +
                $"{Video.PublishedCol}, " +
                $"{Video.ChannelIdCol} " +
                $") values (" +
                $"@{nameof(Video.Id)}, " +
                $"@{nameof(Video.YoutubeId)}, " +
                $"@{nameof(Video.Title)}, " +
                $"@{nameof(Video.Status)}, " +
                $"@{nameof(Video.Published)}, " +
                $"@{nameof(Video.ChannelId)} " +
                $")";
            using (var context = await dbConnectionFactory.Open())
            {
                await context.ExecuteAsync(query, videos);
            }
        }

        public async Task<string[]> CheckIds(string[] videoIds)
        {
            var query =
                $"select {Video.YoutubeIdCol} " +
                $"from {Video.TableName} " +
                $"where {Video.YoutubeIdCol} = any(@{nameof(videoIds)})";
            using (var context = await dbConnectionFactory.Open())
            {
                var foundIds = await context.QueryAsync<string>(query, new { videoIds });
                var notFoundIds = videoIds.Except(foundIds).ToArray();
                return notFoundIds;
            }
        }

        public async Task UpdateVideoStatus(Guid id, VideoStatus newStatus)
        {
            var query =
                $"update {Video.TableName} " +
                $"set {Video.StatusCol} = @{nameof(newStatus)} " +
                $"where {Video.IdCol} = @{nameof(id)} ";
            using (var context = await dbConnectionFactory.Open())
            {
                await context.ExecuteAsync(query, new { id, newStatus });
            }
        }

        public async Task SaveChannel(Channel channel)
        {
            var query =
                $"insert into {Channel.TableName} (" +
                $"{Channel.IdCol}, " +
                $"{Channel.YoutubeIdCol}, " +
                $"{Channel.TitleCol} " +
                $") values (" +
                $"@{nameof(Channel.Id)}, " +
                $"@{nameof(Channel.YoutubeId)}, " +
                $"@{nameof(Channel.Title)} " +
                $") on conflict on constraint {Channel.YoutubeIdConstraint} do nothing ";
            using (var context = await dbConnectionFactory.Open())
            {
                await context.ExecuteAsync(query, channel);
            }
        }
    }
}