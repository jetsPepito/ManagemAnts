using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class User
    {
        public long id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string pseudo { get; set; }
        public string password { get; set; }
    }
}
