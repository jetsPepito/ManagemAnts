using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class ProjectStatsPage
    {
        public Project Project;
        public User LoggedUser;
        public List<Task> Tasks;
        public List<User> Collaborators;
        public List<User> Creators;
        public List<User> Managers;
        public List<User> AllCollaborators;

        public User SelectedCollaborator;
        public bool isInCollaboratorTab;

        public Task GetTask(string id)
        {
            long Id = long.Parse(id);
            return Tasks.Where(x => x.id == Id).FirstOrDefault();
        }
    }
}
