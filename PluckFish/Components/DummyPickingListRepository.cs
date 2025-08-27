using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class DummyPickinglistRepository : IPickingListRepository
    {
        private List<PickingList> _pluklister = new List<PickingList>();

        public void AddPickingList(PickingList plukliste)
        {
            _pluklister.Add(plukliste);
        }

        public void AddProductToPickingList(PickingList plukliste, Item item)
        {
            plukliste.Lines.Add(item);
        }

        public void DeleteProductFromPickingList(PickingList plukliste, Item item)
        {
            plukliste.Lines.Remove(item);
        }

        public List<PickingList> GetAllPickingList()
        {
            throw new NotImplementedException();
        }

        public PickingList GetPickingList(int id)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetPickingListItems(int pickingListId)
        {
            throw new NotImplementedException();
        }

        public void RemovePickingList(PickingList plukliste)
        {
            _pluklister.Remove(plukliste);
        }

        public void UpdateItemInPickingList(PickingList plukliste, Item item, int amount)
        {
            Item? found = plukliste.Lines.FirstOrDefault(i => i.Equals(item));
            if (found != null)
            {
                found.Amount = amount;
            }
        }

        public void UpdatePickingList(PickingList plukliste)
        {
            throw new NotImplementedException();
        }
    }
}