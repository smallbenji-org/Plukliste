using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Caching.Memory;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class CachedProductRepository : IProductRepository
    {
        private readonly IProductRepository productRepository;
        private readonly IMemoryCache memoryCache;

        public CachedProductRepository(IProductRepository productRepository, IMemoryCache memoryCache)
        {
            this.productRepository = productRepository;
            this.memoryCache = memoryCache;
        }

        public void AddProduct(Product product)
        {
            productRepository.AddProduct(product);
            memoryCache.Remove($"product_{product.ProductID}");
            memoryCache.Remove($"product_all");
            memoryCache.Remove($"product_sum");
        }

        public void DeleteProduct(Product product)
        {
            productRepository.DeleteProduct(product);
            memoryCache.Remove($"product_{product.ProductID}");
            memoryCache.Remove($"product_all");
            memoryCache.Remove($"product_sum");
        }

        public IEnumerable<Product> getAll()
        {
            return memoryCache.GetOrCreate($"product_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return productRepository.getAll();
            });
        }

        public Product getProduct(string productId)
        {
            return memoryCache.GetOrCreate($"product_{productId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return productRepository.getProduct(productId);
            });
        }

        public List<Item> GetSumOfUsedItemsInPickingLists()
        {
            return memoryCache.GetOrCreate($"product_sum", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return productRepository.GetSumOfUsedItemsInPickingLists();
            });
        }
    }
}