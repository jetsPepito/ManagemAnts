using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public class Task: IObjectWithId
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Duration { get; set; }
        public int State { get; set; }
        public int? TimeSpent { get; set; }
        public long ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
