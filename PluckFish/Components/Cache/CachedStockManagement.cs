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

        public Item GetItemStock(string prodId)
        {
            //return stockRepository.GetItemStock(prodId);

            // CACHE VIRKER IKKE
            var cached = cache.GetOrCreate($"itemStock_{prodId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return stockRepository.GetItemStock(prodId);
            });

            return new Item
            {
                Product = new Product { Name = cached.Product.Name, ProductID = cached.Product.ProductID},
                Type = cached.Type,
                Amount = cached.Amount,
                RestVare = cached.RestVare
            };
        }

        public List<Item> GetStock(string whereClause = "")
        {
            return cache.GetOrCreate($"getStock_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return stockRepository.GetStock(whereClause);
            });
        }

        public void OrderStock(List<Item> orderedStock)
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

        public void SaveStock(Item savedStock)
        {
            cache.Remove($"itemStock_{savedStock.Product.ProductID}");
            cache.Remove("getStock_all");
            stockRepository.SaveStock(savedStock);
        }

        public bool StockExist(string prodId)
        {
            return stockRepository.StockExist(prodId);
        }
    }
}
