using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IProductRepository
    {
        void AddProduct(Product product);
        void DeleteProduct(Product product);
    }
}