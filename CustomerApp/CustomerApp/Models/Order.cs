using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    class Order
    {
        public IList<MenuFoodItem> Contents { get; set; }


        public string WaitstaffEmail { get; set; }

        public bool sent { get; set; }
    }
}
