using Microsoft.Extensions.Caching.Memory;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class CachedPickingListRepository : IPickingListRepository
    {
        private readonly IPickingListRepository pickingListRepository;
        private readonly IMemoryCache cache;

        public CachedPickingListRepository(IPickingListRepository pickingListRepository, IMemoryCache cache)
        {
            this.pickingListRepository = pickingListRepository;
            this.cache = cache;
        }

        public void AddPickingList(PickingList plukliste)
        {
            pickingListRepository.AddPickingList(plukliste);
            cache.Remove("pickinglist_all");
            cache.Remove($"pickinglist_{plukliste.Id}");
        }

        public void AddProductToPickingList(PickingList plukliste, Item item)
        {
            pickingListRepository.AddProductToPickingList(plukliste, item);
            cache.Remove($"pickinglistitems_{plukliste.Id}");
        }

        public void DeleteProductFromPickingList(PickingList plukliste, Item item)
        {
            pickingListRepository.DeleteProductFromPickingList(plukliste, item);
            cache.Remove($"pickinglistitems_{plukliste.Id}");
        }

        public List<PickingList> GetAllPickingList()
        {
            return cache.GetOrCreate($"pickinglist_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return pickingListRepository.GetAllPickingList();
            });
        }

        public PickingList GetPickingList(int id)
        {
            return cache.GetOrCreate($"pickinglist_{id}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return pickingListRepository.GetPickingList(id);
            });
        }

        public List<Item> GetPickingListItems(int pickingListId)
        {
            return cache.GetOrCreate($"pickinglistitems_{pickingListId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return pickingListRepository.GetPickingListItems(pickingListId);
            });
        }

        public void RemovePickingList(PickingList plukliste)
        {
            pickingListRepository.RemovePickingList(plukliste);
            cache.Remove("pickinglist_all");
            cache.Remove($"pickinglist_{plukliste.Id}");
        }

        public void UpdateItemInPickingList(PickingList plukliste, Item item, int amount)
        {
            pickingListRepository.UpdateItemInPickingList(plukliste, item, amount);
            cache.Remove($"pickinglistitems_{plukliste.Id}");
        }

        public void UpdatePickingList(PickingList plukliste)
        {
            pickingListRepository.UpdatePickingList(plukliste);
            cache.Remove("pickinglist_all");
            cache.Remove($"pickinglist_{plukliste.Id}");
        }
    }
}