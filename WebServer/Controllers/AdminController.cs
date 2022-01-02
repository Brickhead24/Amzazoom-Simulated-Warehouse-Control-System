using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.ViewModels;

namespace WebServer.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Secure(int? authorized)
            //Checks for authorization and proceeds to Admin Control pannel if authorized
        {
            if( authorized == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("IncorrectLogin","Admin");
            }
            

        }

        public ActionResult IncorrectLogin()
            //Displays incorrect login page
        {
            return View();
        }

        #nullable enable
        public ActionResult ItemManager(string? order, string warehouseName)
            //page for adding/removing items from inventory
        {
            var tcpConnection = new TCPClient();
            var viewModel = new ItemManagerViewModel();
            viewModel.order = order;
            viewModel.warehouseName = warehouseName;

            string warehouses = tcpConnection.TCPGetWarehouses();
            //warehouse names are contained in a string of format: warehouseName1,warehouseName2...
            string[] whs = warehouses.Split(',');

           
            foreach (string wh in whs)
            {
                //add all warehouses to be displayed
                viewModel.whNames.Add(wh);
            }

            return View(viewModel);
        }

        public ActionResult SubmitStockingOrder(string order, string warehouseName)
            //Sends order to backend warehouse
        {
            var viewModel = new ResultViewModel();
            if (order != null)
            {                
                var tcp_client = new TCPClient();
                viewModel.result = tcp_client.TCPStockItems(order, warehouseName);
            }
            else
            {
                viewModel.result = "Your Submitted Order Was Empty, Please Try Again";
            }
            
            return View(viewModel);
        }

        public ActionResult RobotManager(string? warehouseName)
            //Page for adding/removing robots
        {
            var viewModel = new RobotViewModel();
            var tcp_client = new TCPClient();           
            string warehouses = tcp_client.TCPGetWarehouses();
            string[] whs = warehouses.Split(',');
            bool validFlag = false;

            viewModel.warehouseName = warehouseName;
            foreach (string wh in whs)
            {
                viewModel.whNames.Add(wh);
                if (warehouseName == wh)
                {
                    validFlag = true;
                }
            }



            if (warehouseName != null)
            {
                if (validFlag == false)
                {
                    return RedirectToAction("InvalidWarehouseInput", "Admin");
                }

                viewModel.qty = tcp_client.TCPGetRobotQty(warehouseName);
            }
            return View(viewModel);
        }

        public ActionResult AddRobot(string warehouseName, int qty)
            //Calls function to add robot to backend warehouse
        {
            var viewModel = new ResultViewModel();
            var tcp_client = new TCPClient();
            viewModel.result=tcp_client.TCPAddRobots(qty,warehouseName);

            return View(viewModel);
        }
        
        public ActionResult WarehouseManager()
            //Displays webpage for creating a new warehouse
        {
            
            return View();
        }

#nullable enable
        public ActionResult WarehouseShelfLayout(string name, int rows, int cols, string? layout)
            //page for setting up warehouse layout
        {
            var viewModel = new WarehouseViewModel();
            viewModel.name = name;
            viewModel.layout = layout;
            viewModel.rows = rows;
            viewModel.cols = cols;
            return View(viewModel);
        }

        public ActionResult AddWarehouse(string warehouseName, int rows, int cols, string layout)
            //page for submitting new warehouse
        {
            var viewModel = new ResultViewModel();
            var tcp_client = new TCPClient();
            viewModel.result = tcp_client.AddWarehouse(layout,rows,cols,warehouseName);
            return View(viewModel);
        }

        public ActionResult InvalidWarehouseInput()
            //page returned on inputing non-existing warehouse
        {
            return View();
        }

        public ActionResult AnimationManager()
            //Page for selecting warehouse to Animate on backend
        {
            var viewModel = new RobotViewModel();
            var tcp_client = new TCPClient();

            string warehouses = tcp_client.TCPGetWarehouses();
            string[] whs = warehouses.Split(',');

            foreach (string wh in whs)
            {
                viewModel.whNames.Add(wh);
            }
            return View(viewModel);
        }
        public ActionResult WHTAselected(string warehouseName)
            //Submits request to change warehouse to animate and displays comfirmation
        {
            var viewModel = new RobotViewModel();
            var tcp_client = new TCPClient();

            viewModel.warehouseName = warehouseName;

            string warehouses = tcp_client.TCPGetWarehouses();
            string[] whs = warehouses.Split(',');
            bool validFlag = false;

            foreach (string wh in whs)
            {
                viewModel.whNames.Add(wh);
                if (warehouseName == wh)
                {
                    validFlag = true;
                }
            }
            if (validFlag)
            {
                tcp_client.TCPAnimateWarehouse(warehouseName);
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("InvalidWarehouseInput", "Admin");
            }
            
        }
    }
}