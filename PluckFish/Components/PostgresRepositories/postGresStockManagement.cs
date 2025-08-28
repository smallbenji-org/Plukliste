using Dapper;
using Npgsql;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Data;
using System.Data.Common;
using System.Net.NetworkInformation;

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

        public List<Item> getStock(string whereClause = "")
        {
            string sql = "SELECT t1.productId AS \"product_id\", t1.name, COALESCE(t2.amount, 0) AS \"amount\", COALESCE(t2.restVare, false) AS \"restVare\" FROM products t1 LEFT JOIN stock t2 ON t2.product_id = t1.productId"+whereClause;
            DataTable tb = DapperHelper.loadTb(sql, dbConnection);
            List<Item> items = DapperHelper.fillItemListFromTb(tb);
            return items;
        }

        public Item getItemStock(string prodId)
        {
            string sql = "SELECT t1.productId AS \"product_id\", t1.name, COALESCE(t2.amount, 0) AS \"amount\", COALESCE(t2.restVare, false) AS \"restVare\" FROM products t1 LEFT JOIN stock t2 ON t2.product_id = t1.productId WHERE t1.productId = @product_id";
            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new
            {
                product_id = prodId
            });
            DataTable tb = new DataTable();
            tb.Load(reader);

            if (tb.Rows.Count <= 0) { return null; }

            DataRow row = tb.Rows[0];
            Item item = new Item();
            item.Amount = int.Parse(row["amount"].ToString());
            if (row["restVare"].ToString().ToLower() == "true")
            {
                item.RestVare = true;
            }
            item.Product = new Product();
            item.Product.ProductID = prodId;
            item.Product.Name = row["name"].ToString();

            return item;
        }
        public bool stockExist(string prodId)
        {
            string sql = "SELECT 1 FROM stock WHERE product_id = @product_id";
            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new
            {
                product_id = prodId
            });
            DataTable tb = new DataTable();
            tb.Load(reader);

            if (tb.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        public void orderStock(List<Item> orderedStock)
        {
            throw new NotImplementedException();
        }

        private void UpdateStockSql(string prodId, int retractNum)
        {
            string sql = $"UPDATE stock SET amount = amount-@retractNum WHERE product_id = @product_id";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new
            {
                product_id = prodId,
                retractNum = retractNum
            });
        }

        public void RetractStock(string prodId, int retractNum)
        {
            Item item = getItemStock(prodId);
            if (item == null) { return; }
            if (item.Amount - retractNum < 0 && item.RestVare == false) { return; }

            UpdateStockSql(prodId, retractNum);
        }

        public bool RetractMultiStock(List<Item> items)
        {
            string sql = "SELECT product_id, amount, restVare from stock where product_id in (";
            Dictionary<string, int> retractItems = new Dictionary<string, int>();
            foreach (Item item in items) 
            {
                sql += "'"+item.Product.ProductID + "',";
                retractItems.Add(item.Product.ProductID, item.Amount);
            }
            sql = sql.Substring(0, sql.Length-1);
            sql += ")";

            DataTable tb = DapperHelper.loadTb(sql, dbConnection);

            if (retractItems.Count != tb.Rows.Count) { return false; } // Some stock items arent creating. We need to handle this somehow.

            foreach (DataRow dr in tb.Rows) 
            {
                bool restVare = bool.Parse(dr["restVare"].ToString());
                int amount = int.Parse(dr["amount"].ToString());

                var kvp = retractItems[dr["product_id"].ToString()];                

                if (amount - kvp < 0 && restVare == false) { return false; }
            }

            foreach (KeyValuePair<string, int> keyValue in retractItems) 
            {
                UpdateStockSql(keyValue.Key,keyValue.Value);
            }

            return true;
        }

        public void saveStock(Item savedStock)
        {
            string sql = $"UPDATE stock SET amount = @amount, restVare = @restVare WHERE product_id = @product_id";
            if (!stockExist(savedStock.Product.ProductID))
            {
                sql = "INSERT INTO stock (product_id, amount, restVare) VALUES (@product_id, @amount, @restVare)";
            }

            using IDbConnection db = dbConnection;
            db.Execute(sql, new
            {
                product_id = savedStock.Product.ProductID,
                amount = savedStock.Amount,
                restVare = savedStock.RestVare
            });
        }
    }
}