using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class RobotManagerTests
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

        Robot TestRobot1 = new Robot(500, new Grid.GridLocation(1, 1), new Grid(5, 5), new object());
        RobotManager TestManager = new RobotManager();

        TestManager.AddRobot(TestRobot1);
        TestManager.ProcessOrder(TestOrder1);

        if (!(TestTruck1.MaxWeight-TestTruck1.WeightRemaining == TestOrder1.Weight)) { throw new Exception("Failed to load order on truck"); }
        if (!(TestRobot1.WeightRemaining == TestRobot1.MaxWeight)) { throw new Exception("Failed to unload order from robot"); }
        if (!(TestRobot1.RobotGridLocation.column == -1 && TestRobot1.RobotGridLocation.row == -1)) { throw new Exception("Failed to return to charging station"); }
        if (!(TestRobot1.OrderAssigned == null)) { throw new Exception("Failed to unassignn order from robot"); }
        if (!(TestOrder1.CurrentOrderStatus == Order.OrderStatus.OnDeliveryTruck)) { throw new Exception("Failed to update order status"); }

        TestManager.StockOrder(TestOrder1);

        //To-do, write stocking tests once stocking functionality is added
    }
}

