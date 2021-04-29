using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest.API
{
    public class BaseSetup
    {
        protected Mock<ManagemAntsDbContext> contextMock;
        protected Fixtures _refFixture;

        protected void SetupDb()
        {
            contextMock = new Mock<ManagemAntsDbContext>();
            var database = new DbSetup();

            contextMock.Setup(s => s.Set<User>()).Returns(database.UsersSet);
            contextMock.Setup(s => s.Users).Returns(database.UsersSet);

            contextMock.Setup(s => s.Set<Project>()).Returns(database.ProjectsSet);
            contextMock.Setup(s => s.Projects).Returns(database.ProjectsSet);

            contextMock.Setup(s => s.Set<Task>()).Returns(database.TasksSet);
            contextMock.Setup(s => s.Tasks).Returns(database.TasksSet);

            contextMock.Setup(s => s.Set<ProjectsHasUser>()).Returns(database.ProjectHasUsersSet);
            contextMock.Setup(s => s.ProjectsHasUsers).Returns(database.ProjectHasUsersSet);

            contextMock.Setup(s => s.Set<UsersHasTask>()).Returns(database.UsersHasTasksSet);
            contextMock.Setup(s => s.UsersHasTasks).Returns(database.UsersHasTasksSet);

            _refFixture = new Fixtures();
        }
    }
}
