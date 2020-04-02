using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    public class User : RealmObject
    {
        //email will serve as the user's unique GUID and primary key for
        //unique user lookup/storage
        public string _id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        [PrimaryKey]
        public string email { get; set; }
        public string password { get; set; }
        public string birthday { get; set; }
    }
}
