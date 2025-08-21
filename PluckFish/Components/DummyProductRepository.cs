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
    }
}