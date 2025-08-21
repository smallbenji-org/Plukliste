using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IPluklisteRepository
    {
        void AddPlukliste(Plukliste plukliste);
        void RemovePlukliste(Plukliste plukliste);
        void AddProductToPlukliste(Plukliste plukliste, Item item);
        void DeleteProductFromPlukliste(Plukliste plukliste, Item item);
        void UpdateItemInPlukliste(Plukliste plukliste, Item item, int amount);
    }
}