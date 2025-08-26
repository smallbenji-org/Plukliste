using Dapper;
using Npgsql;
using PluckFish.Models;
using System.Data;
using System.Data.Common;

namespace PluckFish.Components
{
    public static class DapperHelper
    {
        public static DataTable loadTb(string sql, IDbConnection dbConn)
        {
            using IDbConnection db = dbConn;
            
            var reader = db.ExecuteReader(sql);
            DataTable tb = new DataTable();
            tb.Load(reader);
            return tb;
        }

        public static List<Item> fillItemListFromTb(DataTable tb)
        {
            List<Item> items = new List<Item>();
            foreach (DataRow row in tb.Rows)
            {
                Item item = new Item();
                Product product = new Product();
                product.ProductID = row["product_id"].ToString();
                product.Name = row["name"].ToString();
                item.Product = product;
                
                item.Amount = int.Parse(row["amount"].ToString());

                items.Add(item);
            }
            return items;
        }
    }
}
