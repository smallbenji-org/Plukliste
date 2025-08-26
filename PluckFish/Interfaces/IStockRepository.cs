using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IStockRepository
    {
        List<Item> getStock();
        void saveStock(List<Item> savedStock);
        void orderStock(List<Item> orderedStock);
    }
}
