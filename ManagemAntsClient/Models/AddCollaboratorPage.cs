using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class AddCollaboratorPage
    {
        public string ProjectId;
        public string ProjectName;
        public List<User> Collaborators;
        public User LoggedUser;
    }
}
