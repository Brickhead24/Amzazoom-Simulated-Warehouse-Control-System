using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebServer.Models;
using WebServer.ViewModels;

namespace WebServer.Controllers
{
    public class OrderStatusController : Controller
    {
        // GET: OrderNumber
        public ActionResult Index()
            //Page for requesting item number
        {
            return View();
        }


        public ActionResult Status(string orderNumber)
            //Retrives order information and displays current status
        {
            var viewModel = new OrderStatusViewModel();

            TCPClient tcpConnection = new TCPClient();
            string temp = tcpConnection.TCPGetOrderStatus(orderNumber);
            viewModel.orderStatus = temp;

            return View(viewModel);
        }

    }
}