using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public class User: IObjectWithId
    {
        public User()
        {
            ProjectsHasUsers = new HashSet<ProjectsHasUser>();
        }

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Pseudo { get; set; }
        public string Password { get; set; }

        public virtual ICollection<ProjectsHasUser> ProjectsHasUsers { get; set; }
    }
}
