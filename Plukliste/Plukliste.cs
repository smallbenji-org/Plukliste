namespace Plukliste;
public class Pluklist
{
    public List<Item> items = new List<Item>();
    public void AddItem(Item item) { items.Add(item); }
}

public class Item
{
    public string productid;
    public string title;
    public ItemType type;
    public int amount;
}

public enum ItemType
{
    Fysisk, Print
}



