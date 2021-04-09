using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess.EfModels
{
    public partial class Project
    {
        public Project()
        {
            ProjectsHasUsers = new HashSet<ProjectsHasUser>();
            Tasks = new HashSet<Task>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ProjectsHasUser> ProjectsHasUsers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
