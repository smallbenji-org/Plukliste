using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IProductRepository
    {
        Product getProduct(string productId);
        List<Item> GetSumOfUsedItemsInPickingLists();
        void AddProduct(Product product);
        void DeleteProduct(Product product);
        IEnumerable<Product> getAll();
    }
}