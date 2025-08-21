using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IPickingListRepository
    {
        void AddPickingList(PickingList plukliste);
        void RemovePickingList(PickingList plukliste);
        void AddProductToPickingList(PickingList plukliste, Item item);
        void DeleteProductFromPickingList(PickingList plukliste, Item item);
        void UpdateItemInPickingList(PickingList plukliste, Item item, int amount);
    }
}