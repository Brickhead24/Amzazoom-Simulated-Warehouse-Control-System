using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Collections.Concurrent;

namespace ServerClass
{
    public interface ZoomInterface
    {
        string GetItems(string warehouseName);
        string CreateOrder(string order, string warehouseName);
        string OrderStatus(string orderNumber);
        string GetWarehouses();
    }

    public interface AdminInterface
    {
        string AddRobots(int qty, string warehouseName);
        int GetRobotQty(string warehouseName);
        string AddStock(string order, string warehouseName);

        string AddWarehouse(string layout, int rows, int cols, string warehouseName);

        void SelectWHTA(string warehouseName);
    }


    class ZoomRemoteClass : MarshalByRefObject, ZoomInterface, AdminInterface
    {
        private int callCount = 0;
        public string testString = "\0";
        private static ConcurrentDictionary<string, WarehouseCentralComputer> warehouseDict = new ConcurrentDictionary<string, WarehouseCentralComputer>();
        private static bool startupFlag = true;
        public ZoomRemoteClass()
        {
            //Creates a single Main warehouse on startup with a few items in it
            if (startupFlag)
            {
                warehouseDict.TryAdd("Main_Warehouse", new WarehouseCentralComputer("Main_Warehouse"));

                warehouseDict["Main_Warehouse"].SetPrintToConsoleFlag(true);

                warehouseDict["Main_Warehouse"].AddRobot(1000);
                warehouseDict["Main_Warehouse"].AddRobot(1000);
                warehouseDict["Main_Warehouse"].AddRobot(1000);

                AddDefaultItems("Main_Warehouse");
                startupFlag = false;
            }
            

        }

        private void AddDefaultItems(string warehouseName)
        {
            for(int i = 0; i < 5; i++)
            {
                warehouseDict[warehouseName].AddItemToWareHouseInventory(new Item("Apple", new Item.ItemLocation(3, 3, false, 1), Item.ItemStatus.status1, 2, 1));
                warehouseDict[warehouseName].AddItemToWareHouseInventory(new Item("Banana", new Item.ItemLocation(3, 3, true, 1), Item.ItemStatus.status1, 2, 2));
                warehouseDict[warehouseName].AddItemToWareHouseInventory(new Item("Coconut", new Item.ItemLocation(1, 3, false, 1), Item.ItemStatus.status1, 5, 5));
                warehouseDict[warehouseName].AddItemToWareHouseInventory(new Item("Dairy", new Item.ItemLocation(1, 3, true, 1), Item.ItemStatus.status1, 10, 4));
            }
            
        }

        public string GetWarehouses()
        {
            string warehouses = "";
            int i = 1;
            int size = warehouseDict.Count;
            foreach(KeyValuePair<string,WarehouseCentralComputer> warehouse in warehouseDict)
            {
                //adds each warehouse name to the string
                warehouses += warehouse.Key;
                if (i != size)
                {
                     //adds a ',' to separate warehouse names
                    warehouses += ',';
                }
                
            }
            //result string will be of form: warehouseName1,warehouseName2....
            return warehouses;
        }
        public string GetItems (string warehouseName)
        {
            //Retrieves item list of given warehouse
            Task<string> task = new Task<string>(warehouseDict[warehouseName].GetItems);
            task.Start();
            task.Wait();
            return task.Result;
        }

        public string OrderStatus(string orderNumber)
        {
            //Retrieves orderstatus of given warehouse, separating the warehouse name from the number to determine which warehouse to send to
            string[] values = orderNumber.Split('-');
            Task<string> task = new Task<string>(()=> warehouseDict[values[0]].GetOrderStatusString(Int32.Parse(values[1])));
            task.Start();
            task.Wait();
            return task.Result;
        }
        
        public string CreateOrder(string order,string warehouseName)
        {
            Task<int> task = new Task<int>(() => warehouseDict[warehouseName].CreateOrderNumber());
            task.Start();
            task.Wait();
            int orderNumber = task.Result;
            int qty;
            string name;

            Order temp_order = new Order(orderNumber, Order.OrderStatus.WaitingForRobot);
            Item temp_item;

            /*Extracts data from string
             * '!' separates items
             * ',' separates attributes
             * Form is: item=itemName,qty,price
             * Eg. item=apple,3,1!item=banana,2,1
             */

            string[] items = order.Split('!');
            foreach (string item in items)
            {
                string[] parts = item.Split('=');
                string[] values = parts[1].Split(',');
                name = values[0];
                qty = Int32.Parse(values[1]);

                if (qty <= warehouseDict[warehouseName].CheckItemInventoryCount(name))
                {
                    temp_item = warehouseDict[warehouseName].GetItemInfo(name);
                    for(int j = 0; j < qty; j++)
                    {
                        temp_order.AddItem(temp_item);
                    }
                }
                else
                {
                    return "Order Failed, Please check quantities and try again";
                }
            }
            //sends order to warehouse in new thread
            Thread thread1 = new Thread(() => warehouseDict[warehouseName].ProcessOrder(temp_order));
            thread1.Start();
            //sends success message to frontend
            return "Order Has Been Sucessfully Placed!\nYour Order Number is: " + warehouseName+ '-' + orderNumber.ToString();

        }

        public string AddRobots(int qty, string warehouseName)
        {
            //adds robots if qty<1
            if (qty > 0)
            {
                for (int i = 0; i < qty; i++)
                {
                    Thread thread1 = new Thread(() => warehouseDict[warehouseName].AddRobot(1000));
                    thread1.Start();
                }
                if (qty > 1)
                {
                    return qty.ToString() + " Robots Added Successfully!";
                }
                else if (qty == 1)
                {
                    return qty.ToString() + " Robot Added Successfully!";
                }                    
            }
            //removes robots if qty<0
            else if(qty<0)
            {
                for (int i = 0; i > qty; i--)
                {
                    Thread thread1 = new Thread(warehouseDict[warehouseName].RemoveRobot);
                    thread1.Start();
                }
                if (qty < -1)
                {
                    return qty.ToString() + " Robots Removed Successfully!";
                }
                else if (qty == -1)
                {
                    return qty.ToString() + " Robot Removed Successfully!";
                }           
            }
            
            return "You've entered a quantity change of 0, Robot Quantity unchanged.";
        }

        public int GetRobotQty(string warehouseName)
        {
            //returns robot idle quantity to frontend
            Task<int> task = new Task<int>(() => warehouseDict[warehouseName].getRobotQty());
            task.Start();
            task.Wait();
            return task.Result;
        }

        public string AddStock(string order, string warehouseName)
        {
            string name;
            double price, weight;
            int row, col, shelf, qty;
            bool rSide;
            Item.ItemLocation itemLocation;
            Item temp_item;
            Task<int> task = new Task<int>(() => warehouseDict[warehouseName].CreateOrderNumber());
            task.Start();
            task.Wait();
            int orderNumber = task.Result;
            Order new_order = new Order(orderNumber, Order.OrderStatus.WaitingForRobot);


            /*Extracts data from string
             * '!' separates items
             * ',' separates attributes
             * Form is: item=itemName,weight,price,row,col,shelf,rSide,qty
             */

            string[] items = order.Split('!');
            foreach (string item in items)
            {
                string[] parts = item.Split('=');
                string[] values = parts[1].Split(',');

                name = values[0];
                weight = Double.Parse(values[1]);
                price = Double.Parse(values[2]);
                row = Int32.Parse(values[3]);
                col = Int32.Parse(values[4]);
                shelf = Int32.Parse(values[5]);
                rSide = Boolean.Parse(values[6]);
                qty = Int32.Parse(values[7]);

                
                itemLocation = new Item.ItemLocation(row, col, rSide, shelf);

                temp_item = new Item(name, itemLocation, Item.ItemStatus.status1, weight, price);
                if (qty > 0) {
                    for (int i = 0; i < qty; i++)
                    {
                        new_order.AddItem(temp_item);

                    }
                }
                else if (qty < 0)
                {
                    for (int i = 0; i > qty; i--)
                    {
                        warehouseDict[warehouseName].RemoveItemFromInventory(name);
                    }
                }
                

            }

            //only calls UnloadOrder if there are items to add
            if(new_order.ItemsInOrder() > 0) {
                Thread thread1 = new Thread(() => warehouseDict[warehouseName].UnloadOrder(new_order));
                thread1.Start();
                return "Stocking Order Has Been Sucessfully Placed!\nYour Order Number is:" + warehouseName + '-' + orderNumber.ToString();
            }
            //if no items added, returns different message
            return "Items removed from shelf sucessfully";

        }

        public string AddWarehouse(string layout, int rows, int cols, string warehouseName)
        {
            int row, col;
            string shelfLocations = "";

            /*extracts shelf loactions from layout string
             * '!' separates locations
             * ',' separates cordinates
             * Form is: item=itemName,weight,price,row,col,shelf,rSide,qty
             */
            string[] items = layout.Split('!');


            foreach (string item in items)
            {
                string[] parts = item.Split('=');
                string[] values = parts[1].Split(',');


                row = Int32.Parse(values[0]);
                col = Int32.Parse(values[1]);
                //Creates shelfLoactions string
                //Eg: 5,6 6,7 7,7 creates 3 shelves, at locations (5,6), (6,7), and (7,7) respectively
                if (shelfLocations != "")
                {
                    shelfLocations = shelfLocations + ' ';
                }
                shelfLocations = shelfLocations + row.ToString() + ',' + col.ToString();
                
            }

            if(warehouseDict.TryAdd(warehouseName,new WarehouseCentralComputer(warehouseName,rows, cols, shelfLocations)))
            {
                return "Warehouse Added Sucessfully";
            }
            else
            {
                return "Warehouse Not Added Sucessfully";
            }
            
        }
        public void SelectWHTA(string warehouseName)
        {
            Thread thread1 = new Thread(() => WarehouseToAnimate(warehouseName) );
            thread1.Start();
        }

        private void WarehouseToAnimate(string warehouseName)
        {
            //Sets all warehouses to NOT animate
            foreach (KeyValuePair<string, WarehouseCentralComputer> warehouse in warehouseDict)
            {
                    warehouse.Value.SetPrintToConsoleFlag(false); 
            }
            //sets selected warehouse to animate
            warehouseDict[warehouseName].SetPrintToConsoleFlag(true);
            
        }


    }
}
