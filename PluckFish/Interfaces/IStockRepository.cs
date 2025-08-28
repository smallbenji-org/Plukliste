using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IStockRepository
    {
        List<Item> getStock(string whereClause = "");
        Item getItemStock(string prodId);
        bool stockExist(string prodId);
        void saveStock(Item savedStock);
        void orderStock(List<Item> orderedStock);
        void RetractStock(string prodId, int retractNum);
        bool RetractMultiStock(List<Item> items);
    }
}
