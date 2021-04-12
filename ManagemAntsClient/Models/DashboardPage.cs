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

        public List<User> usersAdded = new List<User>();
    }
}
