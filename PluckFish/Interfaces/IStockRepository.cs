using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IStockRepository
    {
        List<Item> getStock();
        Item getItemStock(string prodId);
        bool stockExist(string prodId);
        void saveStock(Item savedStock);
        void orderStock(List<Item> orderedStock);
    }
}
