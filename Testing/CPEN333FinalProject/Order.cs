using System;
using System.Collections.Generic;
using System.Text;

class Order
{
    private int orderNumber;
    public int OrderNumber => orderNumber;

    private double weight;
    public double Weight => weight;

    private double orderCost;
    public double OrderCost => orderCost;

    private Truck truckAssigned;
    public Truck TruckAssigned => truckAssigned;

    private object orderLock = new object();

    public enum OrderStatus
    {
        //Change names later
        WaitingForRobot,
        RobotGatheringItems,
        OnDeliveryTruck,
        OrderStocked
    }

    public Order(int orderNumber, OrderStatus orderStatus)
    {
        this.orderNumber = orderNumber;
        this.weight = 0;
        currentOrderStatus = orderStatus;
        orderCost = 0;
    }

    private OrderStatus currentOrderStatus;
    public OrderStatus CurrentOrderStatus => currentOrderStatus;

    private List<Item> itemList = new List<Item>();
    public IReadOnlyList<Item> ItemList => itemList.AsReadOnly();

    public void AddItem(Item item)
    {
        weight += item.Weight;
        orderCost += item.ItemCost;
        item.OrderNumber = orderNumber;
        itemList.Add(item);
    }

    public void AssignTruck(Truck truck)
    {
        lock(orderLock){
            truckAssigned = truck;
            truck.AddOrder(this);
        };
    }

    public void AssignOrderStatus(OrderStatus orderStatus)
    {
        lock (orderLock)
        {
            currentOrderStatus = orderStatus;
        };
    }

}