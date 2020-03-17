using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    class Order
    {
        public List<MenuItem> contents { get; set; }

        public double unpaidBalance { get; set; }
    }
}
