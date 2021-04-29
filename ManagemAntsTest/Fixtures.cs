using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest
{
    public class Fixtures
    {
        public List<User> Users;
        public List<Project> Projects;
        public List<ProjectsHasUser> ProjectsHasUsers;
        public List<Task> Task;
        public List<UsersHasTask> UsersHasTasks;

        public Fixtures()
        {
            setUsers();
            setProjects();
            setProjectHasUsers();
        }

        private void setUsers()
        {
            Users = new List<User>
            {
                new User()
                {
                    Id = 1,
                    Firstname = "Toto",
                    Lastname = "Titi",
                    Pseudo = "TT",
                    Password = "1234"
                },
                new User()
                {
                    Id = 2,
                    Firstname = "Tata",
                    Lastname = "Tutu",
                    Pseudo = "TT2",
                    Password = "azerty"
                },
            };
        }

        private void setProjects()
        {
            Projects = new List<Project>
            {
                new Project()
                {
                    Id = 1,
                    Name = "Premier projet",
                    Description = "Une description du premier projet."
                },
                new Project()
                {
                    Id = 2,
                    Name = "Deuxieme projet",
                    Description = "Une description du deuxieme projet."
                }
            };
        }

        private void setProjectHasUsers()
        {
            ProjectsHasUsers = new List<ProjectsHasUser>
            {
                new ProjectsHasUser()
                {
                    Id = 1,
                    ProjectId = 1,
                    UserId = 1,
                    Role = 0
                },
                new ProjectsHasUser()
                {
                    Id = 2,
                    ProjectId = 1,
                    UserId = 2,
                    Role = 1
                },
                new ProjectsHasUser()
                {
                    Id = 3,
                    ProjectId = 2,
                    UserId = 1,
                    Role = 1
                },
                new ProjectsHasUser()
                {
                    Id = 4,
                    ProjectId = 2,
                    UserId = 2,
                    Role = 0
                },
            };
        }
    }
}
