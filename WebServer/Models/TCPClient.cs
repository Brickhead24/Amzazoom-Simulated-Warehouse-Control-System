using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using ServerClass;


namespace WebServer.Models
{
    
    public class TCPClient
    {
        // Create an instance of the remote object.
        //private ZoomRemoteClass service;

        private TcpChannel clientChannel;
        private System.Runtime.Remoting.Messaging.IMessageSink messageSink;
        private string objectUri;
        
        private static bool firstRun = true;
        private ZoomInterface zoomObject;
        private Type zoomType = typeof(ZoomInterface);

        private string serverStr = "tcp://localhost:8085/RemoteZoom";
        public void SetupConnection()
        {
            // Create the channel.
            clientChannel = new TcpChannel();

            // Register the channel.
            ChannelServices.RegisterChannel(clientChannel, false);

            // Register as client for remote object.
            //WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(System.Type.GetType("ServerClass.ZoomRemoteClass, ServerClass"), serverStr);
            //RemotingConfiguration.RegisterWellKnownClientType(remoteType);



            // Create a message sink.

            messageSink = clientChannel.CreateMessageSink(serverStr, null, out objectUri);

            Console.WriteLine("The URI of the message sink is {0}.", objectUri);
            if (messageSink != null)
            {
                Console.WriteLine("The type of the message sink is {0}.", messageSink.GetType().ToString());
            }
            

        }
        public string TCPGetOrderStatus(string orderNumber)
            //Retrieves orderStatus from backend
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }

            zoomObject = (ZoomInterface)Activator.GetObject(zoomType, serverStr);
            return zoomObject.OrderStatus(orderNumber);
        }

        public string TCPGetItems(string warehouseName)
            //Retrieves invenetory list from given backend warehouse
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            zoomObject = (ZoomInterface)Activator.GetObject(zoomType, serverStr);
            return zoomObject.GetItems(warehouseName);
        }

        public string TCPStockItems(string order, string warehouseName)
            //Adds or Removes stock from backend Warehosue
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            AdminInterface adminObject;
            Type adminType = typeof(AdminInterface);
            adminObject = (AdminInterface)Activator.GetObject(adminType, serverStr);
            return adminObject.AddStock(order, warehouseName);
        }
        public int TCPGetRobotQty(string warehouseName)
            //Retrieves current idle robot quanitity at given warehouse
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            AdminInterface adminObject;
            Type adminType = typeof(AdminInterface);
            adminObject = (AdminInterface)Activator.GetObject(adminType, serverStr);
            return adminObject.GetRobotQty(warehouseName);

        }

        public string TCPAddRobots(int qty,string warehouseName)
        //Adds robots to given warehouse
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            AdminInterface adminObject;
            Type adminType = typeof(AdminInterface);
            adminObject = (AdminInterface)Activator.GetObject(adminType, serverStr);
            return adminObject.AddRobots(qty, warehouseName);
        }

        public string SendOrder(string order,string warehouseName)
            //Sends order to given warehouse
        {
            //service = new ZoomRemoteClass();
            //bool response_data = service.SetString("Hello there");
            //Console.WriteLine("Set null returns {0}", service.SetString(null));
            zoomObject = (ZoomInterface)Activator.GetObject(zoomType, serverStr);
            return zoomObject.CreateOrder(order, warehouseName);

        }

        public string AddWarehouse(string layout, int rows, int cols,string warehouseName)
            //Creates warehouse on backend
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            AdminInterface adminObject;
            Type adminType = typeof(AdminInterface);
            adminObject = (AdminInterface)Activator.GetObject(adminType, serverStr);
            return adminObject.AddWarehouse(layout,rows,cols, warehouseName);

        }

        public string TCPGetWarehouses()
            //Retrieves list of warehouse names from backend
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            ZoomInterface zoomObject;
            Type zoomType = typeof(ZoomInterface);
            zoomObject = (ZoomInterface)Activator.GetObject(zoomType, serverStr);
            return zoomObject.GetWarehouses();
        }

        public void TCPAnimateWarehouse(string warehouseName)
            //Sets backend warehouse to animate
        {
            if (firstRun)
            {
                SetupConnection();
                firstRun = false;
            }
            AdminInterface adminObject;
            Type adminType = typeof(AdminInterface);
            adminObject = (AdminInterface)Activator.GetObject(adminType, serverStr);
            adminObject.SelectWHTA(warehouseName);

        }

    }
}