using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class User
    {
        public User()
        {
            role = 2;
        }

        public long id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string pseudo { get; set; }
        public string password { get; set; }

        public int role { get; set; }
    }
}
