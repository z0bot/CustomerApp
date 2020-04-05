using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    public class MenuFoodItem : RealmObject
    {
        public IList<Ingredient> ingredients { get; }

        [PrimaryKey]
        public string _id { get; set; }

        public string name { get; set; }

        public string picture { get; set; }

        public string nutrition { get; set; } // Just manually input formatted text to be displayed in an alert, lmao

        public double price { get; set; }
        public string StringPrice => price.ToString("C");

        public string description { get; set; }

        public string special_instruct { get; set; }

        public bool paid { get; set; }

        public string category { get; set; }

        // Default constructor
        public MenuFoodItem() { }

        // Copy constructor
        public MenuFoodItem(MenuFoodItem m)
        {
            _id = m._id;

            ingredients = m.ingredients;

            name = m.name;

            picture = m.picture;

            nutrition = m.nutrition;

            price = m.price;

            description = m.description;

            special_instruct = m.special_instruct;

            paid = m.paid;

            category = m.category;
        }
    }
}
