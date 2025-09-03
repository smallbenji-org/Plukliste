namespace PluckFish.Models
{
    public class PickingList
    {
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Forsendelse { get; set; }
        public string Adresse { get; set; }
        public bool IsDone { get; set; }
        public List<Item> Lines { get; set; } = new List<Item>();
        public void AddItem(Item item) { Lines.Add(item); }
    }

    public class Item
    {
        public Product Product { get; set; }
        public ItemType Type { get; set; }
        public int Amount { get; set; }
        public bool RestVare = false;
    }

    public enum ItemType
    {
        Fysisk, Print
    }
}