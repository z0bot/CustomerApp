using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    class Order
    {
        public List<MenuItem> Contents { get; set; }

        public double UnpaidBalance { get; set; }

        public string WaitstaffEmail { get; set; }
    }
}
