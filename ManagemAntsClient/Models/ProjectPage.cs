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
        public List<User> Creators;
        public List<User> Mangers;

        public long OpenedTask;
        public bool isMyTasks;

        public Task GetTask(string id)
        {
            long Id = long.Parse(id);
            return Tasks.Where(x => x.id == Id).FirstOrDefault();
        }
    }

}
