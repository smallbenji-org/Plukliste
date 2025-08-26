namespace PluckFish.Models
{
    public class PickingList
    {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Forsendelse { get; set; }
    public string? Adresse { get; set; }

    public List<Item> Lines = new List<Item>();
    public void AddItem(Item item) { Lines.Add(item); }
    }

    public class Item
    {
    public Product? Product { get; set; }
        public ItemType Type;
        public int Amount;
    }

    public enum ItemType
    {
        Fysisk, Print
    }
}