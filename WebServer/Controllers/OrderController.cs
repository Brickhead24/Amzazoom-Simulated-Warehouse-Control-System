using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Web;

using System.Web.Mvc;
using WebServer.Models;
using WebServer.ViewModels;

namespace WebServer.Controllers
{
    public class OrderController : Controller
    {
        #nullable enable
        public ActionResult Index(string? warehouseName)
        {
            TCPClient tcpConnection = new TCPClient();
            var viewModel = new ItemViewModel();
            viewModel.warehouseName = warehouseName;
            string warehouses = tcpConnection.TCPGetWarehouses();
            string[] whs = warehouses.Split(',');
            bool validFlag = false;

            foreach (string wh in whs)
            {
                //add each warehouse returned to a list to be displayed
                viewModel.whNames.Add(wh);
                if(warehouseName == wh)
                {
                    //if input warehouseName exists, set flag to valid
                    validFlag =true;
                }
            }

            

            if (warehouseName != null)
            {
                if (validFlag == false)
                {
                    //if the inputted warehouse did not exist, return an error
                    return RedirectToAction("InvalidWarehouseInput", "Order");
                }
                
                //Get items list from backend
                string input = tcpConnection.TCPGetItems(warehouseName);
                string temp_name = "";
                int temp_avail = 0;
                double temp_price = 0;
                //Input item string of format: Item=name1,qtyAvailible,price!Item=name,qtyAvailible,price...

                //Function below extracts item info from string
                if (input != "")
                {
                    string[] items = input.Split('!');
                    foreach (string item in items)
                    {
                        string[] parts = item.Split('=');
                        string[] values = parts[1].Split(',');

                        temp_name = values[0];
                        temp_avail = Int32.Parse(values[1]);
                        temp_price = Double.Parse(values[2]);
                        //add item from backend to viewModel if in stock
                        if (temp_avail > 0)
                        {
                            viewModel.CreateItem(temp_name, temp_avail, temp_price);
                        }
                        

                    }
                }
            }
            return View(viewModel);
        }

        public ActionResult InvalidWarehouseInput()
            //Displays error if invalid warehouseName is input
        {
            return View();
        }


        public ActionResult Confirmation(string input,string warehouseName)
            //Sends order to backend warehosue and displays result
        {
            TCPClient tcpConnection = new TCPClient();
            

            string OrderResponse = tcpConnection.SendOrder(input,warehouseName);

            var viewModel = new ConfirmationViewModel();

            viewModel.result = OrderResponse;

            /*Extracts data from string
             * '!' separates items
             * ',' separates attributes
             * Form is: item=itemName,qty,price
             * Eg. item=apple,3,1!item=banana,2,1
             */

            string[] items = input.Split('!');
            foreach (string item in items)
            {
                string[] parts = item.Split('=');
                string[] values = parts[1].Split(',');
                string name = values[0];
                int qty = Int32.Parse(values[1]);
                if (qty > 0)
                {
                    viewModel.CreateItem(name, qty, 0);
                }
                

            }



            return View(viewModel);
        }
    }
}