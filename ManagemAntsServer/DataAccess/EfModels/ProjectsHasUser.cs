using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess.EfModels
{
    public partial class ProjectsHasUser
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public int Role { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
