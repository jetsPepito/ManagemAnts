using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public class ProjectsHasUser: IObjectWithId
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public int Role { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
