using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    class Order : RealmObject
    {
        [PrimaryKey]
        public string _id { get; set; }

        public IList<OrderItem> menuItems { get; }


        public string waitstaff_id { get; set; }

        public bool send_to_kitchen { get; set; }

        public Order() { }

        // Copy constructor
        public Order(Order o)
        {
            _id = o._id;
            // Deep copy of each menu item
            foreach (OrderItem m in o.menuItems)
                menuItems.Add(new OrderItem(m));

            waitstaff_id = o.waitstaff_id;
            send_to_kitchen = o.send_to_kitchen;
        }

    }
}
