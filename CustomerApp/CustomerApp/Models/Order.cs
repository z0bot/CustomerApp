using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    class Order : RealmObject
    {
        [PrimaryKey]
        public int tableNum { get; set; }

        public IList<MenuFoodItem> Contents { get; }


        public string waitstaff_id { get; set; }

        public bool sent { get; set; }

        public Order() { }

        // Copy constructor
        public Order(Order o)
        {
            tableNum = o.tableNum;
            // Deep copy of each menu item
            foreach (MenuFoodItem m in o.Contents)
                Contents.Add(new MenuFoodItem(m));

            waitstaff_id = o.waitstaff_id;
            sent = o.sent;
        }

    }
}
