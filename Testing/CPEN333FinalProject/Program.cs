using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        WarehouseCentralComputer centralComputer = new WarehouseCentralComputer();

        Console.WriteLine("Enter Row number of item1");
        int item1RowNumber = Int32.Parse(Console.ReadLine());
        Console.WriteLine("Enter Column number of item1");
        int item1ColumnNumber = Int32.Parse(Console.ReadLine());
        Item.ItemLocation item1Location = new Item.ItemLocation(item1RowNumber, item1ColumnNumber, false, 1);

        Console.WriteLine("Enter weight of item1");
        double item1Weight = double.Parse(Console.ReadLine());

        Item item1 = new Item("item1", item1Location, Item.ItemStatus.status1, item1Weight, 100);

        Console.WriteLine("Enter Row number of item2");
        int item2RowNumber = Int32.Parse(Console.ReadLine());
        Console.WriteLine("Enter Column number of item2");
        int item2ColumnNumber = Int32.Parse(Console.ReadLine());
        Item.ItemLocation item2Location = new Item.ItemLocation(item2RowNumber, item2ColumnNumber, true, 1);

        Console.WriteLine("Enter weight of item2");
        double item2Weight = double.Parse(Console.ReadLine());

        Item item2 = new Item("Item2", item2Location, Item.ItemStatus.status1, item2Weight, 100);

        Order order1 = new Order(1, Order.OrderStatus.WaitingForRobot);
        order1.AddItem(item1);

        Order order2 = new Order(2, Order.OrderStatus.WaitingForRobot);
        order2.AddItem(item2);

        centralComputer.AddRobot(1000);
        centralComputer.AddRobot(1000);

        Thread thread1 = new Thread(() => centralComputer.ProcessOrder(order1));
        Thread thread2 = new Thread(() => centralComputer.UnloadOrder(order2));

        Order order3 = new Order(3, Order.OrderStatus.WaitingForRobot);
        order3.AddItem(item2);
        order3.AddItem(item1);
        Thread thread3 = new Thread(() => centralComputer.UnloadOrder(order3));

        //thread1.Start();
        Thread.Sleep(500);
        thread2.Start();
        Thread.Sleep(500);
        thread3.Start();


    }


}
