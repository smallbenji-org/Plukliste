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

        Item IStockRepository.getItemStock(string prodId)
        {
            return cache.GetOrCreate($"itemStock_{prodId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return stockRepository.getItemStock(prodId);
            });
        }

        List<Item> IStockRepository.getStock(string whereClause)
        {
            return cache.GetOrCreate($"getStock_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return stockRepository.getStock(whereClause);
            });
        }

        void IStockRepository.orderStock(List<Item> orderedStock)
        {
            throw new NotImplementedException();
        }

        bool IStockRepository.RetractMultiStock(List<Item> items)
        {
            foreach(Item item in items)
            {
                cache.Remove($"itemStock_{item.Product.ProductID}");
            }

            return stockRepository.RetractMultiStock(items);
        }

        void IStockRepository.RetractStock(string prodId, int retractNum)
        {
            cache.Remove($"itemStock_{prodId}");
            stockRepository.RetractStock(prodId, retractNum);
        }

        void IStockRepository.saveStock(Item savedStock)
        {
            cache.Remove($"itemStock_{savedStock.Product.ProductID}");
            stockRepository.saveStock(savedStock);
        }

        bool IStockRepository.stockExist(string prodId)
        {
            return stockRepository.stockExist(prodId);
        }
    }
}
