using System;
using System.Collections.Generic;
using System.Text;
class Item
{

    private string name;
    public string Name => name;

    public struct ItemLocation
    {
        public ItemLocation(int row, int column, bool rightSide, int shelfNumber)
        {
            this.column = column;
            this.row = row;
            this.rightSide = rightSide;
            this.shelfNumber = shelfNumber;
        }
        public int column;
        public int row;
        public bool rightSide;
        public int shelfNumber;

    }

    private ItemLocation currentItemLocation;
    public ItemLocation CurrentItemLocation => currentItemLocation;

    public enum ItemStatus
    {
        //Change names later
        status1,
        status2,
        status3
    }

    private ItemStatus currentItemStatus;
    public ItemStatus CurrentItemStatus
    {
        get => currentItemStatus;
        set => currentItemStatus = value;
    }

    private double weight;
    public double Weight => weight;

    private double itemCost;
    public double ItemCost => itemCost;

    public Item()
    {
        this.name = "";
        this.currentItemStatus = ItemStatus.status1;
        this.weight = 0;
    }

    public Item(string name, ItemLocation currentItemLocation, ItemStatus currentItemStatus, double weight, double itemCost)
    {
        this.name = name;
        this.currentItemLocation = currentItemLocation;
        this.currentItemStatus = currentItemStatus;
        this.weight = weight < 0 ? 0 : weight;
        this.itemCost = itemCost;
    }

    private int orderNumber;
    public int OrderNumber
    {
        get => orderNumber;
        set => orderNumber = value;
    }

}