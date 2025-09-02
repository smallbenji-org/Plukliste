using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class DummyProductRepository : IProductRepository
    {
        private List<Product> products = new List<Product>();

        public void AddProduct(Product product)
        {
            products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            products.Remove(product);
        }

        public IEnumerable<Product> getAll()
        {
            return products;
        }

        public Product getProduct(string productId)
        {
            return products.Where(x => x.ProductID.Equals(productId)).FirstOrDefault();
        }

        public List<Item> GetSumOfUsedItemsInPickingLists()
        {
            throw new NotImplementedException();
        }
    }
}