using Npgsql;
using Dapper;
using System.Data;

namespace PluckFish
{
    public class PostGres
    {
        private readonly IConfiguration configuration;

        public PostGres(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IDataReader ExecuteReader(string query)
        {
            using var conn = new NpgsqlConnection(configuration.GetConnectionString("defaultConnection"));
            return conn.ExecuteReader(query);
        }
        public object? ExecuteScalar(string query, object param = null)
        {
            using var conn = new NpgsqlConnection(configuration.GetConnectionString("defaultConnection"));
            return conn.ExecuteScalar(query, param);
        }
        public object? Execute(string query, object param = null)
        {
            using var conn = new NpgsqlConnection(configuration.GetConnectionString("defaultConnection"));
            return conn.Execute(query, param);
        }
    }
}
