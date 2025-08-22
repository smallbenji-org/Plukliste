using Dapper;
using Npgsql;

namespace PluckFish.Components
{
    public class PostgresEnsureTables
    {
        private readonly IConfiguration config;

        public PostgresEnsureTables(IConfiguration config)
        {
            this.config = config;
        }


        public void Ensure()
        {
            using var conn = new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS picking_lists (
                    id SERIAL PRIMARY KEY,
                    name TEXT,
                    forsendelse TEXT,
                    adresse TEXT
                );

                CREATE TABLE IF NOT EXISTS picking_list_items (
                    id SERIAL PRIMARY KEY,
                    picking_list_id INTEGER REFERENCES picking_lists(id) ON DELETE CASCADE,
                    product_id TEXT,
                    type INTEGER,
                    amount INTEGER
                );

                CREATE TABLE IF NOT EXISTS products (
                    productId TEXT PRIMARY KEY,
                    name TEXT
                );
            ");
        }
    }
}