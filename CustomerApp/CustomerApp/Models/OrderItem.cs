using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    // This class is very similar to MenuFoodItem, but is used to store items which are in the user's order
    public class OrderItem : RealmObject
    {
        [PrimaryKey]
        public string newID { get; set; }

        public IList<string> ingredients { get; }

        public string _id { get; set; } // Original item's ID

        public bool couponApplied { get; set; } = false;
        

        public string name { get; set; }

        public double price { get; set; }
        public string StringPrice => price.ToString("C");

        public string special_instruct { get; set; }

        public bool paid { get; set; }

        public bool prepared { get; set; }

        // Default constructor
        public OrderItem() { }

        // Create item based on Menu Item
        public OrderItem(MenuFoodItem m)
        {
            _id = m._id;

            foreach(Ingredient i in m.ingredients)
                ingredients.Add(i._id);

            name = m.name;

            price = m.price;

            special_instruct = m.special_instruct;

            paid = m.paid;
        }

        // Copy constructor
        public OrderItem(OrderItem o)
        {
            if (o == null)
                return;

            _id = o._id;

            ingredients = o.ingredients;

            name = o.name;

            price = o.price;

            special_instruct = o.special_instruct;

            paid = o.paid;
        }
    }
}
