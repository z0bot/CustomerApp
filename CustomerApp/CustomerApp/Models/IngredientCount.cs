using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models
{
    public class IngredientCount : RealmObject
    {
        [PrimaryKey]
        public string _id { get; set; }
        public int quantity { get; set; }
        

    }
}
