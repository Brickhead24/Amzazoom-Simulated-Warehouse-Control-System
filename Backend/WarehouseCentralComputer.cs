using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;

class WarehouseCentralComputer
{
    private string warehouseName;
    public string WarehouseName => warehouseName;

    private const int DEFAULT_ROW_SIZE = 5;
    private const int DEFAULT_COLUMN_SIZE = 9;

    private List<Truck> deliveryTruckList = new List<Truck>();
    private ConcurrentQueue<Truck> restockTruckQueue = new ConcurrentQueue<Truck>();

    private Dictionary<int, Order> orderDictionary = new Dictionary<int, Order>(); 
    private RobotManager robotManager = new RobotManager();
    private InventoryManager inventoryManager = new InventoryManager();
    private Grid grid;

    private readonly object printLock = new object();
    private readonly object unloadLock = new object();

    public WarehouseCentralComputer(string warehouseName)
    {
        this.warehouseName = warehouseName;
        SetUpDefaultWarehouse();
    }

    /*
    * @param warehouseName - Name of warehouse
    * @param numberOfRows - Row size of warehouse
    * @param numberOfColumns - Column size of warehouse
    * @param shelfCoordinates - space separated string of ware house coordintes. I.e.: "1,2 1,3 3,2" puts shelves at 
    *                           (row 1, column 2), (row 1, column 3), (row 3, column 2) 
    */
    public WarehouseCentralComputer(string warehouseName, int numberOfRows, int numberOfColumns, string shelfCoordinates)
    {
        this.warehouseName = warehouseName;
        SetupCustomWarehouse(numberOfRows, numberOfColumns, shelfCoordinates.Split(' '));
    }

    private void SetUpDefaultWarehouse()
    {
        grid = new Grid(warehouseName,DEFAULT_ROW_SIZE, DEFAULT_COLUMN_SIZE);

        grid.SetUnWalkable(1, 1, false);
        grid.SetUnWalkable(2, 1, false);
        grid.SetUnWalkable(3, 1, false);

        grid.SetUnWalkable(1, 3, false);
        grid.SetUnWalkable(2, 3, false);
        grid.SetUnWalkable(3, 3, false);

        grid.SetUnWalkable(1, 5, false);
        grid.SetUnWalkable(2, 5, false);
        grid.SetUnWalkable(3, 5, false);

        grid.SetUnWalkable(1, 7, false);
        grid.SetUnWalkable(2, 7, false);
        grid.SetUnWalkable(3, 7, false);

        AddDeliveryTrucks();
    }

    private void SetupCustomWarehouse(int numberOfRows, int numberOfColumns, string[] shelfCoordinates)
    {
        grid = new Grid(warehouseName,numberOfRows, numberOfColumns);
        for (int i = 0; i < shelfCoordinates.Length; i++)
        {
            string[] coordinate = shelfCoordinates[i].Split(',');
            grid.SetUnWalkable(Int32.Parse(coordinate[0]), Int32.Parse(coordinate[1]), false);
        }
        AddDeliveryTrucks();
    }

    public void ProcessOrder(Order order) {

        orderDictionary.Add(order.OrderNumber, order);
        RemoveItemsFromInventory(order.ItemList);

        //1. Assign Truck
        AssignTruckToOrder(order);
        //2. OrderQueue -> process Order
        robotManager.ProcessOrder(order);
    }

    private void RemoveItemsFromInventory(IReadOnlyList<Item> items)
    {
        foreach(Item item in items)
        {
            inventoryManager.RemoveItemFromInventory(item.Name);
        }
    }

    public void RemoveItemFromInventory(string name)
    {
        inventoryManager.RemoveItemFromInventory(name);
    }

    public void UnloadOrder(Order order)
    {
        orderDictionary.Add(order.OrderNumber, order);
        lock (unloadLock)
        {
            Grid.GridLocation truckGridLocation = new Grid.GridLocation(0, 2);
            Truck truck = new Truck(10000, truckGridLocation);
            order.AssignTruck(truck);
            robotManager.StockOrder(order);
            inventoryManager.UpdateInventoryFromIncomingTruck(order);
        }
    }

    public void AddDeliveryTrucks()
    {
        Grid.GridLocation truckGridLocation1 = new Grid.GridLocation(0, 3);
        Grid.GridLocation truckGridLocation2 = new Grid.GridLocation(0, 4);
        Truck truck1 = new Truck(10000, truckGridLocation1);
        Truck truck2 = new Truck(10000, truckGridLocation2);
        deliveryTruckList.Add(truck1);
        deliveryTruckList.Add(truck2);
    }

    public void AddRobot(Robot robot)
    {
        robotManager.AddRobot(robot);
    }

    public void AddRobot(double maxWeight)
    {
        Grid.GridLocation robotGridLocation = new Grid.GridLocation(-1, -1);
        Robot robot = new Robot(maxWeight, robotGridLocation, grid, printLock);
        robotManager.AddRobot(robot);
    }

    public void RemoveRobot()
    {
        robotManager.RemoveRobot();
    }

    public int getRobotQty()
    {
        return robotManager.GetQty();
    }
    private void AssignTruckToOrder(Order order)
    {
        foreach(Truck truck in deliveryTruckList)
        {
            if (truck.WeightRemaining >= order.Weight)
            {
                order.AssignTruck(truck);
                return;
            }
        }
        deliveryTruckList.Clear();
        AddDeliveryTrucks();
        order.AssignTruck(deliveryTruckList[0]);

    }

    public Order.OrderStatus GetOrderStatus(int orderNumber)
    {
        return orderDictionary[orderNumber].CurrentOrderStatus;
    }

    public string GetOrderStatusString(int orderNumber)
    {
        return orderDictionary[orderNumber].CurrentOrderStatus.ToString();
    }

    
    public void AddItemToWareHouseInventory(Item item)
    {
        inventoryManager.AddItem(item);
    }
    

    public int CheckItemInventoryCount(string name)
    {
        return inventoryManager.GetItemCount(name);
    }

    /*
    public void RemoveItemFromInventory(string name)
    {
        inventoryManager.RemoveItemFromInventory(name);
    }
    */

    public string GetItems()
    {
        return inventoryManager.GetItems();
    }
    public Item GetItemInfo(string name)
    {
        return inventoryManager.GetItemInfo(name);
    }
    public int CreateOrderNumber()
    {
        return orderDictionary.Count + 1;
    }
    public void SetPrintToConsoleFlag(bool printToConsole)
    {
        grid.SetPrintToConsoleFlag(printToConsole);
    }



}
