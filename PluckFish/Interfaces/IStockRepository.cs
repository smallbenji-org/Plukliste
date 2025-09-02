using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IStockRepository
    {
        List<Item> GetStock(string whereClause = "");
        List<Product> GetBareboneProductsInStock();
        Item GetItemStock(string prodId);
        bool StockExist(string prodId);
        void SaveStock(Item savedStock);
        void OrderStock(List<Item> orderedStock);
        void RetractStock(string prodId, int retractNum);
        bool RetractMultiStock(List<Item> items);
    }
}
