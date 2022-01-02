using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class OrderTests
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
        TestOrder1.AssignOrderStatus(Order.OrderStatus.OrderStocked);
        TestOrder1.AssignTruck(TestTruck1);

        if (!(TestOrder1.CurrentOrderStatus == Order.OrderStatus.OrderStocked)) { throw new Exception("Wrong order status"); }
        if (!(TestOrder1.ItemList.Count == 3)) { throw new Exception("Wrong number of items"); }
        if (!(TestOrder1.OrderCost == 2*TestItem1.ItemCost+TestItem2.ItemCost)) { throw new Exception("Wrong order cost"); }
        if (!(TestOrder1.OrderNumber == 1)) { throw new Exception("Wrong order number"); }
        if (!(TestOrder1.Weight == 2 * TestItem1.Weight + TestItem2.Weight)) { throw new Exception("Wrong weight"); }
        if (!(TestOrder1.TruckAssigned.MaxWeight - TestOrder1.TruckAssigned.WeightRemaining == 2 * TestItem1.Weight + TestItem2.Weight)) { throw new Exception("Wrong weight"); }
    }
}

