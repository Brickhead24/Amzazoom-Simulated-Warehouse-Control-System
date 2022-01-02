using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class InventoryManagerTests
{
    static void Main()
    {
        Item.ItemLocation TestLocation1 = new Item.ItemLocation(1, 1, true, 1);
        Item.ItemStatus TestItemStatus1 = Item.ItemStatus.status1;
        Item TestItem1 = new Item("TestItem1", TestLocation1, TestItemStatus1, 1, 1);

        Item.ItemLocation TestLocation2 = new Item.ItemLocation(2, 2, false, 2);
        Item.ItemStatus TestItemStatus2 = Item.ItemStatus.status3;
        Item TestItem2 = new Item("TestItem2", TestLocation2, TestItemStatus2, 5, 5);

        Order TestOrder1 = new Order(1, Order.OrderStatus.RobotGatheringItems);
        TestOrder1.AddItem(TestItem2);

        InventoryManager TestManager1 = new InventoryManager();

        TestManager1.AddItem(TestItem1);
        TestManager1.AddItem(TestItem2);
        TestManager1.AddItem(TestItem2);
        if (!ItemCompare(TestManager1.GetItemInfo(TestItem1.Name), TestItem1)) { throw new Exception("Error retrieving Item"); }
        if (!ItemCompare(TestManager1.GetItemInfo(TestItem2.Name), TestItem2)) { throw new Exception("Error retrieving Item"); }
        if (!(TestManager1.GetItemCount(TestItem1.Name) == 1)) { throw new Exception("Unexpected number of items"); }
        if (!(TestManager1.GetItemCount(TestItem2.Name) == 2)) { throw new Exception("Unexpected number of items"); }
        TestManager1.RemoveItemFromInventory(TestItem2.Name, 1);
        if (!(TestManager1.GetItemCount(TestItem2.Name) == 1)) { throw new Exception("Unexpected number of items"); }
        TestManager1.UpdateInventoryFromIncomingTruck(TestOrder1);
        if (!(TestManager1.GetItemCount(TestItem2.Name) == 2)) { throw new Exception("Unexpected number of items"); }

    }

    static bool ItemCompare(Item item1, Item item2)
    {
        if (item1.Name==item2.Name && item1.ItemCost==item2.ItemCost && item1.OrderNumber==item2.OrderNumber && item1.Weight==item2.Weight
            && item1.CurrentItemLocation.column==item2.CurrentItemLocation.column  && item1.CurrentItemLocation.rightSide== item2.CurrentItemLocation.rightSide 
            && item1.CurrentItemLocation.row== item2.CurrentItemLocation.row && item1.CurrentItemLocation.shelfNumber== item2.CurrentItemLocation.shelfNumber
            && item1.CurrentItemStatus== item2.CurrentItemStatus)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

