using Dapper;
using Npgsql;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class PostgresAuthRepository : IAuthRepository
    {
        private readonly IConfiguration config;

        private NpgsqlConnection Connection => new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));

        public PostgresAuthRepository(IConfiguration config)
        {
            this.config = config;
        }

        public List<ApplicationUser> GetAll()
        {
            using var db = Connection;
            var sql = "SELECT Id, username, full_name FROM users";

            return db.Query<ApplicationUser>(sql).ToList();
        }
    }
}
