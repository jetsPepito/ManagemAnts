using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess.EfModels
{
    public partial class Task
    {
        public Task()
        {
            UsersHasTasks = new HashSet<UsersHasTask>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Duration { get; set; }
        public int State { get; set; }
        public int? TimeSpent { get; set; }
        public long ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<UsersHasTask> UsersHasTasks { get; set; }
    }
}
