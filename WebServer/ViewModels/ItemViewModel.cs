using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models;

namespace WebServer.ViewModels
{
    public class ItemViewModel
    {
        public List<WebItem> Items = new List<WebItem>();
        public string warehouseName = "";
        public string warehouses;
        public List<string> whNames = new List<string>();

        public void AddItem(WebItem item)
        {
            Items.Add(item);
        }

        public void CreateItem(string name, int avail, double price)
        {
            WebItem item = new WebItem();
            item.name = name;
            item.inv = avail;
            item.price = price;

            Items.Add(item);

            
        }


    }
}