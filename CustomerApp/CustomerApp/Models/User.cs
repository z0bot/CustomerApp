using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.Models
{
    class User : RealmObject
    {
        //email will serve as the user's unique GUID and primary key for
        //unique user lookup/storage
        [PrimaryKey]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Birthday { get; set; }

        public string points { get; set; }

        public int tableNum { get; set; }
    }
}
