using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.ViewModels
{
    public class ItemManagerViewModel
    {
        public string order { get; set; }
        public string warehouseName;
        public List<string> whNames = new List<string>();
    }
}