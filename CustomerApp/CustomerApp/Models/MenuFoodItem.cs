using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    public class MenuFoodItem : RealmObject
    {
        [PrimaryKey]
        public string Name { get; set; }

        public string Picture { get; set; }

        public string Nutrition { get; set; } // Just manually input formatted text to be displayed in an alert, lmao

        public double Price { get; set; }

        public string Description { get; set; }

        public string SpecialInstructions { get; set; }

    }
}
