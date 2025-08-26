using Dapper;
using Npgsql;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Data;

namespace PluckFish.Components
{
    public class postGresStockManagement : IStockRepository
    {
        private readonly PostGres postGres;
        private readonly IConfiguration config;

        public postGresStockManagement(PostGres postGres, IConfiguration config)
        {
            this.postGres = postGres;
            this.config = config;
        }

        private IDbConnection dbConnection => new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

        public List<Item> getStock()
        {
            string sql = "SELECT t1.product_id, t1.amount FROM stock t1 INNER JOIN products t2 ON t2.productId = t1.product_id";
            DataTable tb = DapperHelper.loadTb(sql, dbConnection);
            List<Item> items = DapperHelper.fillItemListFromTb(tb);
            return items;
        }

        public void orderStock(List<Item> orderedStock)
        {
            throw new NotImplementedException();
        }

        public void saveStock(List<Item> savedStock)
        {
            foreach (Item item in savedStock) 
            {
                string sql = $"UPDATE stock SET amount = @amount WHERE product_id = @product_id";
                using IDbConnection db = dbConnection;
                db.Execute(sql, new
                {                  
                    product_id = item.Product.ProductID,
                    amount = item.Amount
                });
            }
        }
    }
}
