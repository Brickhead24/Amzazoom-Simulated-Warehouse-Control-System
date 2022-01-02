using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class InventoryManager
{
    //private List<Item> itemlist = new List<Item>();
    private ConcurrentDictionary<string, int> itemCount = new ConcurrentDictionary<string, int>();
    private ConcurrentDictionary<string, Item> itemDictonary = new ConcurrentDictionary<string, Item>();

    public void AddItem(Item item)
    {
        //itemlist.Add(item);

        if (itemCount.ContainsKey(item.Name))
        {
            itemCount[item.Name]++;
        }
        else
        {
            if (!itemCount.TryAdd(item.Name, 1)) 
            {
                throw new Exception("Error, item name already exists");
            }
        }

        if (!itemDictonary.ContainsKey(item.Name))
        {
            if (!itemDictonary.TryAdd(item.Name, item))
            {
                throw new Exception("Error, item name already exists");
            }
        }
    }

    public void UpdateInventoryFromIncomingTruck(Order order)
    {
        foreach (Item item in order.ItemList)
        {
            AddItem(item);
        }
    }

    public int GetItemCount(String name)
    {
        return itemCount[name];
    }

    public void RemoveItemFromInventory(String name, int numberToRemove = 1)
    {
        if (!itemCount.ContainsKey(name))
        {
            return;
        }
        itemCount[name] -= numberToRemove;
    }

    public Item GetItemInfo(string itemName)
    {
        if (itemDictonary.ContainsKey(itemName))
        {
            return itemDictonary[itemName];
        }
        return null;
    }

    public string GetItems()
    {
        string DataString = "";
        int j = 0;
        Item item = new Item();
        int qty = 0;

        foreach (KeyValuePair<string, int> invItem in itemCount)
        {
            //Creates string item=name,qty,price
            qty = invItem.Value;
            item = GetItemInfo(invItem.Key);
            DataString += "item=" + item.Name + ',' + qty + ',' + item.ItemCost;

            //Adds a '!' to separate multiple items
            //Eg. item=apple,3,1!item=banana,2,2
            if (j < itemCount.Count - 1)
            {
                DataString += '!';
            }
            j++;
        }

        return DataString;
    }
}

