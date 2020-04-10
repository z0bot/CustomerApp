using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    class Coupon : RealmObject
    {
        [PrimaryKey]
        public string _id { get; set; }

        public IList<string> requiredItems { get; set; }

        public IList<string> appliedItems { get; set; }

        public string couponType { get; set; }

        public double discount { get; set; }

        public bool repeatable { get; set; }
    }
}
