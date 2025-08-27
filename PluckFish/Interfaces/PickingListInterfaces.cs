using PluckFish.Models;

namespace PluckFish.Interfaces
{
    public interface IPickingListRepository
    {
        void UpdatePickingList(PickingList plukliste);
        void AddPickingList(PickingList plukliste);
        void RemovePickingList(PickingList plukliste);
        void AddProductToPickingList(PickingList plukliste, Item item);
        void DeleteProductFromPickingList(PickingList plukliste, Item item);
        void UpdateItemInPickingList(PickingList plukliste, Item item, int amount);
        List<PickingList> GetAllPickingList();
        PickingList GetPickingList(int id);
        List<Item> GetPickingListItems(int pickingListId);
    }
}