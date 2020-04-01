using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    class Order
    {
        public List<MenuFoodItem> Contents { get; set; }

        public double UnpaidBalance { get; set; }

        public string WaitstaffEmail { get; set; }
    }
}
