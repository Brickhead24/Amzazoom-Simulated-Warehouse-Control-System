using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

class Robot
{
    private bool showSimulation = true;

    private double maxWeight;
    public double MaxWeight => maxWeight;

    private double currentWeightOnRobot;

    public double WeightRemaining => maxWeight - currentWeightOnRobot;

    private List<Item> itemList = new List<Item>();

    private Order orderAssigned;
    public Order OrderAssigned
    {
        get => orderAssigned;
        set => orderAssigned = value;
    }

    private object printLock = new object();

    public enum RobotStatus
    {
        //Change names later
        Idle,
        GettingOrderItems,
        GoingToTruck,
        StockingItems
    }

    private RobotStatus currentRobotStatus;
    public RobotStatus CurrentRobotStatus => currentRobotStatus;

    private Grid.GridLocation robotGridLocation;
    public Grid.GridLocation RobotGridLocation => robotGridLocation;

    private Grid grid;

    public Robot(double maxWeight, Grid.GridLocation startingLocation, Grid grid, object printLock)
    {
        this.maxWeight = maxWeight;
        robotGridLocation = startingLocation;
        currentWeightOnRobot = 0;
        this.grid = grid;
        this.printLock = printLock;
        currentRobotStatus = RobotStatus.Idle;
    }

    public void LoadOrderOnTruck(Order order)
    {
        foreach (Item item in order.ItemList)
        {
            //Go to truck first if robot doesn't have enough room left
            if (currentWeightOnRobot + item.Weight > maxWeight)
            {
                GoToTruck(order.TruckAssigned);
            }
            GoToItem(item);
        }
        GoToTruck(order.TruckAssigned);
    }

    public void UnloadTruck(Order order)
    {
        foreach (Item item in order.ItemList)
        {
            //Stock items first if robot doesn't have enough room left
            if (currentWeightOnRobot + item.Weight > maxWeight)
            {
                StockItems();
                GoToTruck(order.TruckAssigned);
            }
            itemList.Add(item);
            currentWeightOnRobot += item.Weight;
        }
        StockItems();
        GoToTruck(order.TruckAssigned);
    }

    public void ReturnToChargingStation ()
    {
        grid.SetHasRobot(robotGridLocation.row, robotGridLocation.column, false);
        robotGridLocation.row = -1;
        robotGridLocation.column = -1;

        currentRobotStatus = RobotStatus.Idle;

        lock (printLock)
        {
            
            grid.PrintGridToConsole(false);
            Thread.Sleep(1000);
        }
    }

    public void GoToItem(Item item)
    {
        currentRobotStatus = RobotStatus.GettingOrderItems;

        currentWeightOnRobot += item.Weight;
        itemList.Add(item);

        int itemColumnCoordinate = item.CurrentItemLocation.rightSide ? item.CurrentItemLocation.column + 1 : item.CurrentItemLocation.column - 1;

        List <PathNode> pathToItem =
            grid.FindPath(robotGridLocation.row, robotGridLocation.column, item.CurrentItemLocation.row, itemColumnCoordinate);
        if (pathToItem != null)
        {
            foreach (PathNode pathNode in pathToItem)
            {
                if (pathNode.HasRobot)
                {
                    Thread.Sleep(1000);
                }
                pathNode.SetHasRobot(true);
                if (showSimulation)
                {
                    lock (printLock)
                    {

                        grid.PrintGridToConsole(false);
                        Thread.Sleep(1000);
                    }
                }
                robotGridLocation.row = pathNode.row;
                robotGridLocation.column = pathNode.column;
                pathNode.SetHasRobot(false);
            }
        }
    }

    private void StockItems()
    {
        currentRobotStatus = RobotStatus.StockingItems;
        foreach (Item item in itemList)
        {

            int itemColumnCoordinate = item.CurrentItemLocation.rightSide ? item.CurrentItemLocation.column + 1 : item.CurrentItemLocation.column - 1;

            List<PathNode> pathToItem =
                grid.FindPath(robotGridLocation.row, robotGridLocation.column, item.CurrentItemLocation.row, itemColumnCoordinate);
            if (pathToItem != null)
            {
                foreach (PathNode pathNode in pathToItem)
                {
                    if (pathNode.HasRobot)
                    {
                        Thread.Sleep(1000);
                    }
                    pathNode.SetHasRobot(true);
                    if (showSimulation)
                    {
                        lock (printLock)
                        {

                            grid.PrintGridToConsole(true);
                            Thread.Sleep(1000);
                        }
                    }
                    robotGridLocation.row = pathNode.row;
                    robotGridLocation.column = pathNode.column;
                    pathNode.SetHasRobot(false);
                }
            }
            

            currentWeightOnRobot -= item.Weight;
        }
        itemList.Clear();
    }

    public void SetPositionToOrigin()
    {
        while (grid.GetNode(0, 0).HasRobot) { }
        robotGridLocation.row = 0;
        robotGridLocation.column = 0;
        Thread.Sleep(1000);
    }

    public void GoToTruck(Truck truck)
    {
        currentRobotStatus = RobotStatus.GoingToTruck;

        List<PathNode> pathToTruck =
        grid.FindPath(robotGridLocation.row, robotGridLocation.column,
        truck.TruckGridLocation.row, truck.TruckGridLocation.column);

        //PathNode previousNode = null;
        if (pathToTruck != null)
        {
            foreach (PathNode pathNode in pathToTruck)
            {
                if (pathNode.HasRobot)
                {
                    Thread.Sleep(1000);
                }
                pathNode.SetHasRobot(true);
                if (showSimulation)
                {
                    lock (printLock)
                    {
                        Console.Clear();
                        grid.PrintGridToConsole(false);
                        Thread.Sleep(1000);
                    }
                }
                robotGridLocation.row = pathNode.row;
                robotGridLocation.column = pathNode.column;
                pathNode.SetHasRobot(false);
            }
        }
        
        currentWeightOnRobot = 0;
        itemList.Clear();
    }
}

