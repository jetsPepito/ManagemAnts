using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class DashboardPage
    {
        public User LoggedUser;
        public Projects Projects;

        public string projectName;
        public string projectDescription;

        public string addedMessage;

        public List<User> addedCollaborators = new List<User>();
    }
}
