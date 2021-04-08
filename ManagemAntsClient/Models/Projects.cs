using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class Projects
    {
        public IEnumerable<Project> projects;

        public Projects(IEnumerable<Project> pro)
        {
            projects = pro;
        }
    }
}
