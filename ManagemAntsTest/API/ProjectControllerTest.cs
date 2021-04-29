using NUnit.Framework;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsServer.Controllers;
using System;
using AutoMapper;
using ManagemAntsServer.DataAccess.EfModels;
using System.Collections.Generic;

namespace ManagemAntsTest.API
{
    class ProjectControllerTest : BaseSetup
    {
        private ProjectController _projectController;

        [SetUp]
        public void Setup()
        {
            SetupDb();
            var myProfile = new AutomapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            ProjectRepository projectRepository = new ProjectRepository(contextMock.Object, null, mapper);
            UsersHasTaskRepository usersHasTaskRepository = new UsersHasTaskRepository(contextMock.Object, null, mapper);
            ProjectsHasUserRepository projectsHasUserRepository = new ProjectsHasUserRepository(contextMock.Object, null, mapper);
            TaskRepository taskRepository = new TaskRepository(contextMock.Object, null, mapper);
            _projectController = new ProjectController(projectRepository, projectsHasUserRepository, taskRepository, usersHasTaskRepository);
        }

        [Test]
        public void GetProjects()
        {
            var result = _projectController.Get() as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual(2, projects.Length);
        }

        [Test]
        public void GetProjectsById()
        {
            var result = _projectController.GetById("1") as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual("Premier projet", projects[0].Name);
        }

/*        [Test]
        public void Post()
        {
            var newProj = new ManagemAntsServer.Dbo.Project()
            {
                Id = 3,
                Name = "Nouveau projet",
                Description = "La description du nouveau projet"
            };
            var result = _projectController.Post(newProj) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual("Premier projet", projects[0].Name);
        }*/
    }
}
