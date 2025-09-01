using Microsoft.Extensions.Caching.Memory;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components.Cache
{
    public class CachedStockManagement : IStockRepository
    {
        private readonly IStockRepository stockRepository;
        private readonly IMemoryCache cache;

        public CachedStockManagement(IStockRepository stockRepository, IMemoryCache cache)
        {
            this.stockRepository = stockRepository;
            this.cache = cache;
        }

        public Item getItemStock(string prodId)
        {
            return stockRepository.getItemStock(prodId);
            // CACHE VIRKER IKKE
            return cache.GetOrCreate($"itemStock_{prodId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return stockRepository.getItemStock(prodId);
            });
        }

        public List<Item> getStock(string whereClause = "")
        {
            return cache.GetOrCreate($"getStock_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return stockRepository.getStock(whereClause);
            });
        }

        public void orderStock(List<Item> orderedStock)
        {
            throw new NotImplementedException();
        }

        public bool RetractMultiStock(List<Item> items)
        {
            cache.Remove("getStock_all");
            foreach(Item item in items)
            {
                cache.Remove($"itemStock_{item.Product.ProductID}");
            }

            return stockRepository.RetractMultiStock(items);
        }

        public void RetractStock(string prodId, int retractNum)
        {
            cache.Remove($"itemStock_{prodId}");
            cache.Remove("getStock_all");
            stockRepository.RetractStock(prodId, retractNum);
        }

        public void saveStock(Item savedStock)
        {
            cache.Remove($"itemStock_{savedStock.Product.ProductID}");
            cache.Remove("getStock_all");
            stockRepository.saveStock(savedStock);
        }

        public bool stockExist(string prodId)
        {
            return stockRepository.stockExist(prodId);
        }
    }
}
