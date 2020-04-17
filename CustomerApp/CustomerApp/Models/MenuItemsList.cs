using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Realms;

namespace CustomerApp.Models
{
    public class MenuItemsList : RealmObject
    {
        public IList<MenuFoodItem> menuItems { get; }
    }
}
