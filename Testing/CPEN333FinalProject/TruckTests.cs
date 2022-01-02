using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class TruckTests
{
    static void Main()
    {
        Item.ItemLocation TestLocation1 = new Item.ItemLocation(1, 1, true, 1);
        Item.ItemStatus TestItemStatus1 = Item.ItemStatus.status1;
        Item TestItem1 = new Item("TestItem1", TestLocation1, TestItemStatus1, 1, 1);

        Item.ItemLocation TestLocation2 = new Item.ItemLocation(2, 2, false, 2);
        Item.ItemStatus TestItemStatus2 = Item.ItemStatus.status3;
        Item TestItem2 = new Item("TestItem2", TestLocation2, TestItemStatus2, 5, 5);

        Truck TestTruck1 = new Truck(1000, new Grid.GridLocation(1,1));
        Order TestOrder1 = new Order(1, Order.OrderStatus.RobotGatheringItems);

        TestOrder1.AddItem(TestItem1);
        TestOrder1.AddItem(TestItem1);
        TestOrder1.AddItem(TestItem2);
        TestTruck1.AddOrder(TestOrder1);

        if (!(TestTruck1.MaxWeight - TestTruck1.WeightRemaining == 2 * TestItem1.Weight + TestItem2.Weight)) { throw new Exception("Wrong weight"); }
    }
}

