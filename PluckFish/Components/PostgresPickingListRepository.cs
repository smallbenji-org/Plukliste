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
            string sql = "INSERT INTO picking_lists (name, forsendelse, adresse) VALUES (@name, @forsendelse, @adresse) RETURNING id";
            using IDbConnection db = dbConnection;
            plukliste.Id = db.QuerySingle<int>(sql, new { name = plukliste.Name, forsendelse = plukliste.Forsendelse, adresse = plukliste.Adresse });
        }

        public void AddProductToPickingList(PickingList plukliste, Item item)
        {
            string sql = "INSERT INTO picking_list_items (picking_list_id, product_id, type, amount) VALUES (@listId, @productId, @type, @amount)";
            using IDbConnection db = dbConnection;
            db.Execute(sql, new {
                listId = plukliste.Id,
                productId = item.Product?.ProductID,
                type = (int)item.Type,
                amount = item.Amount
            });
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
    }
}