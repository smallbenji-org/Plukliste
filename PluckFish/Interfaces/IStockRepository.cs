using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IStockRepository
    {
        List<Item> getStock();
        void saveStock(Item savedStock);
        void orderStock(List<Item> orderedStock);
    }
}
