using System;
using System.Collections.Generic;
using System.Text;

using Realms;

namespace CustomerApp.Models
{
    // Class used to persistently store whether a user has claimed their birthday gift
    class BirthdayBool : RealmObject
    {
        public bool birthdayGiftClaimed { get; set; } = false;
    }
}
