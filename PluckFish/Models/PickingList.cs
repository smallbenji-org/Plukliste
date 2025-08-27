namespace PluckFish.Models
{
    public class PickingList
    {
        public int Id { get; set; }
        public string? Name;
        public string? Forsendelse;
        public string? Adresse;
        public List<Item> Lines = new List<Item>();
        public void AddItem(Item item) { Lines.Add(item); }
    }

    public class Item
    {
        public Product? Product { get; set; }
        public ItemType Type;
        public int Amount;
        public bool RestVare = false;
    }

    public enum ItemType
    {
        Fysisk, Print
    }
}