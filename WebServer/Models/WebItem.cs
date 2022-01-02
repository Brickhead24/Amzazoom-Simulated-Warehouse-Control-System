using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public class WebItem
    {

        public string name = "";
        public int inv { get; set; }

        public double price { get; set; }




    }

    public class TestOrder
    {
        public Dictionary<string, int> items = new Dictionary<string, int>();
    }
}