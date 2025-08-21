using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Components
{
    public class DummyPluklisteRepository : IPluklisteRepository
    {
        private List<Plukliste> _pluklister = new List<Plukliste>();

        public void AddPlukliste(Plukliste plukliste)
        {
            _pluklister.Add(plukliste);
        }

        public void AddProductToPlukliste(Plukliste plukliste, Item item)
        {
            plukliste.Lines.Add(item);
        }

        public void DeleteProductFromPlukliste(Plukliste plukliste, Item item)
        {
            plukliste.Lines.Remove(item);
        }

        public void RemovePlukliste(Plukliste plukliste)
        {
            _pluklister.Remove(plukliste);
        }

        public void UpdateItemInPlukliste(Plukliste plukliste, Item item, int amount)
        {
            var found = plukliste.Lines.FirstOrDefault(i => i.Equals(item));
            if (found != null)
            {
                found.Amount = amount;
            }
        }
    }
}