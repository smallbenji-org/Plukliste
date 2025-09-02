using System.Data;
using Dapper;
using Npgsql;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish {
    public class PostgresProductRepository : IProductRepository
    {
        private readonly PostGres postGres;
        private readonly IConfiguration config;

        public PostgresProductRepository(PostGres postGres, IConfiguration config)
        {
            this.postGres = postGres;
            this.config = config;
        }

        private IDbConnection dbConnection => new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

        public void AddProduct(Product product)
        {
            string sql = "INSERT INTO products (productId, name) VALUES (@id, @name)";

            using IDbConnection db = dbConnection;

            db.Execute(sql, new { id = product.ProductID, name = product.Name });
        }

        public void DeleteProduct(Product product)
        {
            string sql = "DELETE FROM products WHERE productId = @id";

            using IDbConnection db = dbConnection;

            db.Execute(sql, new { id = product.ProductID });
        }

        public Product getProduct(string productId)
        {
            string sql = "SELECT productId, name FROM products WHERE productId = @id";

            using IDbConnection db = dbConnection;

            return db.QueryFirst<Product>(sql, new { id = productId });
        }

        public IEnumerable<Product> getAll()
        {
            string sql = "SELECT productId, name FROM products";

            using IDbConnection db = dbConnection;
            return db.Query<Product>(sql);
        }

        public List<Item> GetSumOfUsedItemsInPickingLists()
        {
            string sql = @"
               SELECT
               t1.product_id,
               t2.name,
               SUM(amount) as sum1
               FROM picking_list_items t1
               LEFT JOIN products t2 ON t1.product_id = t2.productid
               GROUP BY t1.product_id, t2.name
            ";

            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new { });
            DataTable tb = new DataTable();
            tb.Load(reader);

            List<Item> items = new List<Item>();
            foreach (DataRow row in tb.Rows)
            {
                items.Add(new Item
                {
                    Product = new Product { ProductID = row["product_id"].ToString(), Name = row["name"].ToString() },
                    Amount = int.Parse(row["sum1"].ToString())
                });

            }

            return items;
        }
    }
}