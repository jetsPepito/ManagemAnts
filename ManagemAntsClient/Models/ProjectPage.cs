using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class ProjectPage
    {
        public Project Project;
        public User LoggedUser;
        public List<Task> Tasks;
        public List<User> Collaborators;

        public Task OpenedTask = new Task();

        public Task GetTask(string id)
        {
            long Id = long.Parse(id);
            return Tasks.Where(x => x.id == Id).FirstOrDefault();
        }
    }

}
