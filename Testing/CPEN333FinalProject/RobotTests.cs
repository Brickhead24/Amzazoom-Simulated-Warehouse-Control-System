using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

class RobotTests
{
    static void Main()
    {
        Item.ItemLocation TestLocation1 = new Item.ItemLocation(1, 1, true, 1);
        Item.ItemStatus TestItemStatus1 = Item.ItemStatus.status1;
        Item TestItem1 = new Item("TestItem1", TestLocation1, TestItemStatus1, 1, 1);

        Item.ItemLocation TestLocation2 = new Item.ItemLocation(2, 2, false, 2);
        Item.ItemStatus TestItemStatus2 = Item.ItemStatus.status3;
        Item TestItem2 = new Item("TestItem2", TestLocation2, TestItemStatus2, 5, 5);

        Truck TestTruck = new Truck(1000, new Grid.GridLocation(1,1));

        Order TestOrder1 = new Order(1, Order.OrderStatus.RobotGatheringItems);

        Robot TestRobot1 = new Robot(500, new Grid.GridLocation(1, 1), new Grid(5, 5), new object());

        TestOrder1.AddItem(TestItem1);
        TestOrder1.AddItem(TestItem1);
        TestOrder1.AddItem(TestItem2);
        TestOrder1.AssignOrderStatus(Order.OrderStatus.OrderStocked);
        TestOrder1.AssignTruck(TestTruck);

        TestRobot1.GoToItem(TestItem1);
        if (!(TestRobot1.RobotGridLocation.column == TestItem1.CurrentItemLocation.column + 1 //add 1 as it is on the right side
            && TestRobot1.RobotGridLocation.row == TestItem1.CurrentItemLocation.row)) { throw new Exception("Failed pathing to item location"); }
        TestRobot1.ReturnToChargingStation();
        if (!(TestRobot1.RobotGridLocation.column == -1 && TestRobot1.RobotGridLocation.row == -1)) { throw new Exception("Failed to return to charging station"); }
    }
}

