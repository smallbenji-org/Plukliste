namespace PluckFish.Models
{
    public class PickingList
    {
        public string? Name;
        public string? Forsendelse;
        public string? Adresse;
        public List<Item> Lines = new List<Item>();
        public void AddItem(Item item) { Lines.Add(item); }
    }

    public class Item
    {
        public Product product { get; set; }
        public ItemType Type;
        public int Amount;
    }

    public enum ItemType
    {
        Fysisk, Print
    }
}