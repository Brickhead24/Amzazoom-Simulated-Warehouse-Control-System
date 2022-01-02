using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

class RobotManager
{

    private ConcurrentBag<Order> orderList = new ConcurrentBag<Order>();
    private ConcurrentBag<Order> stockList = new ConcurrentBag<Order>();

    private ConcurrentQueue<Robot> robotList = new ConcurrentQueue<Robot>();

    public void ProcessOrder(Order order)
    {
        //Step1 add to orderList
        orderList.Add(order);

        //Step2 Wait for available robot
        while (robotList.Count == 0) { }
        Robot robot;
        if (!robotList.TryDequeue(out robot))
        {
            throw new Exception("Error, was not able to retrieve robot");
        }
        robot.OrderAssigned = order;

        //Step3 set Order Status
        order.AssignOrderStatus(Order.OrderStatus.RobotGatheringItems);

        robot.SetPositionToOrigin();

        //step 4 Robot starts loading order onto truck
        robot.LoadOrderOnTruck(order);

        order.AssignOrderStatus(Order.OrderStatus.OnDeliveryTruck);

        //Robot returns to charging station
        robot.ReturnToChargingStation();

        //Put robot back in robotList
        robot.OrderAssigned = null;
        robotList.Enqueue(robot);                       
    }

    public void AddRobot(Robot robot)
    {
        robotList.Enqueue(robot);
    }

    public void RemoveRobot()
    {
        Robot robot;
        if (!robotList.TryDequeue(out robot))
        {
            throw new Exception("Error, was not able to retrieve robot");
        }
    }

    public void StockOrder(Order order)
    {
        stockList.Add(order);

        //Step2 Wait for available robot
        while (robotList.Count == 0) { }
        Robot robot;
        if (!robotList.TryDequeue(out robot))
        {
            throw new Exception("Error, was not able to retrieve robot");
        }
        robot.OrderAssigned = order;

        //Step3 set Order Status
        order.AssignOrderStatus(Order.OrderStatus.RobotGatheringItems);

        robot.SetPositionToOrigin();

        //step 4 Robot starts loading order onto truck
        robot.UnloadTruck(order);

        order.AssignOrderStatus(Order.OrderStatus.OrderStocked);

        //Robot returns to charging station
        robot.ReturnToChargingStation();

        //Put robot back in robotList
        robot.OrderAssigned = null;
        robotList.Enqueue(robot);

    }

}

