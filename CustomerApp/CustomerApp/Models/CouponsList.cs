using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    class CouponsList : RealmObject
    {
        public IList<Coupon> Coupons { get; }

    }
}
