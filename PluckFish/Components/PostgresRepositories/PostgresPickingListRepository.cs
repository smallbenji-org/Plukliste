using System.Data;
using Dapper;
using Npgsql;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class PostgresPickingListRepository : IPickingListRepository
    {
        private readonly PostGres postGres;
        private readonly IConfiguration config;

        public PostgresPickingListRepository(PostGres postGres, IConfiguration config)
        {
            this.postGres = postGres;
            this.config = config;
        }

        private IDbConnection dbConnection => new NpgsqlConnection(config.GetConnectionString("defaultConnection"));

        public void AddPickingList(PickingList plukliste)
        {
            string sql = "INSERT INTO picking_lists (name, forsendelse, adresse, isDone) VALUES (@name, @forsendelse, @adresse, @isDone) RETURNING id";
            using IDbConnection db = dbConnection;
            plukliste.Id = db.QuerySingle<int>(sql, new { name = plukliste.Name, forsendelse = plukliste.Forsendelse, adresse = plukliste.Adresse , isDone = plukliste.IsDone});
        }

        public void AddProductToPickingList(PickingList plukliste, Item item)
        {
            List<Item> pickingListItems = GetPickingListItems(plukliste.Id);
            var selectedItems = pickingListItems
            .Where(x => x.Product.ProductID == item.Product.ProductID)
            .ToList();

            using IDbConnection db = dbConnection;
            if (selectedItems.Count > 0) 
            {
                string sql = "UPDATE picking_list_items SET amount = amount+@amount WHERE picking_list_id = @listId AND product_Id = @productId";
                db.Execute(sql, new
                {
                    listId = plukliste.Id,
                    productId = item.Product?.ProductID,
                    amount = item.Amount
                });
            }
            else
            {
                string sql = "INSERT INTO picking_list_items (picking_list_id, product_id, type, amount) VALUES (@listId, @productId, @type, @amount)";
                db.Execute(sql, new
                {
                    listId = plukliste.Id,
                    productId = item.Product?.ProductID,
                    type = (int)item.Type,
                    amount = item.Amount
                });
            }           
        }

        public void DeleteProductFromPickingList(PickingList plukliste, Item item)
        {
            string sql = "DELETE FROM picking_list_items WHERE picking_list_id = @listId AND product_id = @productId";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new {
                listId = plukliste.Id,
                productId = item.Product?.ProductID
            });
        }

        private List<PickingList> getPickingLists(DataTable tb)
        {
            List<PickingList> pickingLists = new List<PickingList>();
            foreach (DataRow row in tb.Rows)
            {
                PickingList pickingList = new PickingList();
                pickingList.Id = int.Parse(row["id"].ToString());
                pickingList.Name = row["name"].ToString();
                pickingList.Adresse = row["adresse"].ToString();
                pickingList.Forsendelse = row["forsendelse"].ToString();
                pickingList.IsDone = bool.Parse(row["isDone"].ToString());
                //pickingList.Lines = GetPickingListItems(pickingList.Id);

                pickingLists.Add(pickingList);
            }
            return pickingLists;
        }

        public List<PickingList> GetAllPickingList()
        {
            string sql = "SELECT id, name, forsendelse, adresse, isDone FROM picking_lists";
            DataTable tb = DapperHelper.loadTb(sql, dbConnection);
            return getPickingLists(tb);
        }

        public int GetSumOfItemInAllPickingLists(string productId)
        {
            string sql = "SELECT SUM(amount) AS \"sum\" FROM picking_list_items WHERE product_id = @productId";
            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new
            {
                productId = productId
            });

            DataTable tb = new DataTable();
            tb.Load(reader);

            if (tb.Rows.Count > 0) {
                DataRow row = tb.Rows[0];
                string test = row["sum"].ToString();
                int.TryParse(row["sum"].ToString(), out int sum);
                return sum;
            }

            return 0;
        }

        public PickingList GetPickingList(int id)
        {
            string sql = "SELECT id, name, forsendelse, adresse, isDone FROM picking_lists WHERE id = @id";
            DataTable tb = DapperHelper.loadTb(sql, dbConnection, new { id = id });
            List<PickingList> pickingLists = getPickingLists(tb);
            return pickingLists[0];
        }

        public List<Item> GetPickingListItems(int id)
        {          
            string sql = "SELECT t1.picking_list_id, t1.product_id, t1.type, t1.amount, t2.name FROM picking_list_items t1 INNER JOIN products t2 ON t2.productId = t1.product_id WHERE t1.picking_list_id = @id";
            using IDbConnection db = dbConnection;
            var reader = db.ExecuteReader(sql, new
            {
                id = id,
            });

            DataTable tb = new DataTable();
            tb.Load(reader);
            List<Item> Items = DapperHelper.fillItemListFromTb(tb);           
            return Items;
        }

        public void RemovePickingList(PickingList plukliste)
        {
            string sql = "DELETE FROM picking_lists WHERE id = @id";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new { id = plukliste.Id });
        }

        public void UpdateItemInPickingList(PickingList plukliste, Item item, int amount)
        {
            string sql = "UPDATE picking_list_items SET amount = @amount WHERE picking_list_id = @listId AND product_id = @productId";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new {
                listId = plukliste.Id,
                productId = item.Product?.ProductID,
                amount = amount
            });
        }

        public void UpdatePickingList(PickingList plukliste)
        {
            string sql = "UPDATE picking_lists SET name = @name, forsendelse = @forsendelse, adresse = @adresse, isDone = @isDone WHERE id = @id";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new { 
                id = plukliste.Id,
                name = plukliste.Name,
                forsendelse = plukliste.Forsendelse,
                adresse = plukliste.Adresse,
                isDone = plukliste.IsDone
            });
        }
    }
}