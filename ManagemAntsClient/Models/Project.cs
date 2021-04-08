using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class Project
    {
        public Project()
        {
            tasks = new HashSet<Task>();
        }

        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public long owner { get; set; }

        public virtual ICollection<Task> tasks { get; set; }
    }
}
