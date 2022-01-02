using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.ViewModels
{
    public class RobotViewModel
    {
        public int qty { get; set; }
        public string warehouseName;
        public List<string> whNames = new List<string>();
    }
}