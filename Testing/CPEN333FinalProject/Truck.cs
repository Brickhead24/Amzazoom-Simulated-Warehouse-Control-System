using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

class Truck
{
    private double maxWeight;
    public double MaxWeight => maxWeight;

    private double currentWeightOnTruck;

    private ConcurrentBag<Order> orders = new ConcurrentBag<Order>();

    public double WeightRemaining => maxWeight - currentWeightOnTruck;

    private Grid.GridLocation truckGridLocation;
    public Grid.GridLocation TruckGridLocation => truckGridLocation;

    public enum TruckStatus
    {
        //Change names later
        Status1,
        Status2,
        Status3
    }

    private TruckStatus currentTruckStatus;
    public TruckStatus CurrentTruckStatus => currentTruckStatus;

    public Truck(double maxWeight, Grid.GridLocation startingLocation)
    {
        this.truckGridLocation = startingLocation;
        this.maxWeight = maxWeight;
        currentWeightOnTruck = 0;
        currentTruckStatus = TruckStatus.Status1;
    }

    public void AddOrder(Order order)
    {
        orders.Add(order);
        currentWeightOnTruck += order.Weight;
    }
}

