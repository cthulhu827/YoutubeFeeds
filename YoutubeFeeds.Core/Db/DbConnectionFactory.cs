using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace YoutubeFeeds.Core
{
    /// <summary>
    /// Менеджер подключений к БД.
    /// </summary>
    public class DbConnectionFactory
    {
        private readonly string connectionString;

        public DbConnectionFactory(IDbSettings settings)
        {
            connectionString = settings.DbConnectionString;
        }

        public async Task<IDbConnection> Open()
        {
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}