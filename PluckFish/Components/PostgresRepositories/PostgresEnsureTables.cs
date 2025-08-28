using System.Data;
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
            using IDbConnection conn = new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS picking_lists (
                    id SERIAL PRIMARY KEY,
                    name TEXT,
                    forsendelse TEXT,
                    adresse TEXT,
                    isDone BOOLEAN DEFAULT FALSE
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

                CREATE TABLE IF NOT EXISTS roles (
                    id TEXT PRIMARY KEY,
                    name TEXT,
                    normalized_name TEXT,
                    description TEXT
                );

                CREATE TABLE IF NOT EXISTS users (
                    id TEXT PRIMARY KEY,
                    username TEXT,
                    normalized_username TEXT,
                    email TEXT,
                    password_hash TEXT,
                    full_name TEXT
                );

                CREATE TABLE IF NOT EXISTS stock (
                    product_id TEXT PRIMARY KEY REFERENCES products(productId) ON DELETE CASCADE,
                    amount INTEGER NOT NULL DEFAULT 0,
                    restVare BOOLEAN NOT NULL DEFAULT FALSE
                );
            ");
        }
    }
}