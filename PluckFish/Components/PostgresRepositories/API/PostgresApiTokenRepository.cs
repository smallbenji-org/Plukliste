using Dapper;
using Npgsql;
using PluckFish.Interfaces.API;
using PluckFish.Models.API;
using System.Data;
using System.Data.Common;

namespace PluckFish.Components.PostgresRepositories.API
{
    public class PostgresApiTokenRepository : IVerificationApi
    {

        private readonly PostGres postGres;
        private readonly IConfiguration config;

        public PostgresApiTokenRepository(PostGres postGres, IConfiguration config)
        {
            this.postGres = postGres;
            this.config = config;
        }

        private IDbConnection dbConnection => new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

        public void createToken(string userId)
        {

            
            if (userId == null || userId == "") { return; }

            string token = Guid.NewGuid().ToString();
            string sql = "INSERT INTO apiTokens (userId, token) VALUES (@userId, @token)";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new
            {
                userId = userId,
                token = token
            });
        }

        public bool Verify(string token)
        {
            // Getting all API Tokens
            List<ApiToken> tokens = GetApiTokens();
            if (tokens == null || tokens.Count == 0) { return false; }

            if (tokens.Any(x => x.Token == token && x.ExpirationDate > DateTime.UtcNow))
            {
                return true; // Acces granted | Token exists and not expired
            }

            return false; // Fallback to false
        }

        public List<ApiToken> GetApiTokens()
        {
            List<ApiToken> tokens = new List<ApiToken>();

            string sql = "SELECT token, expirationDate FROM apiTokens";
            DataTable tb = DapperHelper.loadTb(sql, dbConnection);

            if (tb.Rows.Count > 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    tokens.Add(new ApiToken(row["token"].ToString(), DateTime.Parse(row["expirationDate"].ToString())));
                }
            }

            return tokens;
        }

        public List<ApiToken> GetApiTokensForUser(string userId) 
        {
            List<ApiToken> tokens = new List<ApiToken>();

            string sql = "SELECT token, expirationDate FROM apiTokens WHERE userId = @userId";
            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new
            {
                userId = userId
            });
            DataTable tb = new DataTable();
            tb.Load(reader);

            if (tb.Rows.Count > 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    tokens.Add(new ApiToken(row["token"].ToString(), DateTime.Parse(row["expirationDate"].ToString())));
                }
            }

            return tokens;
        }

        public void RemoveApiTokenFromUser(string userId, string token)
        {
            List<ApiToken> tokens = new List<ApiToken>();

            string sql = "DELETE FROM apiTokens WHERE userId = @userId AND token = @token";
            using IDbConnection db = dbConnection;
            var reader = db.Execute(sql, new
            {
                userId = userId,
                token = token
            });
        }
    }
}
