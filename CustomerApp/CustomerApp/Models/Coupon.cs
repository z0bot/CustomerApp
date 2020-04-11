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

        public IList<string> requiredItems { get; }

        public IList<string> appliedItems { get; }

        public string couponType { get; set; }

        public double discount { get; set; }

        public bool repeatable { get; set; }
    }
}
