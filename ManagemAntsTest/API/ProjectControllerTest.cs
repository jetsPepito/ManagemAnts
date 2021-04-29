using NUnit.Framework;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsServer.Controllers;
using System;
using AutoMapper;
using ManagemAntsServer.DataAccess.EfModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

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
            var result = _projectController.Get() as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual(2, projects.Length);
        }

        [Test]
        public void GetProjectsById()
        {
            var result = _projectController.GetById("1") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual("Premier projet", projects[0].Name);
        }

        [Test]
        public async System.Threading.Tasks.Task Post_Project()
        {
            var newProj = new ManagemAntsServer.Dbo.Project()
            {
                Id = 3,
                Name = "Nouveau projet",
                Description = "La description du nouveau projet"
            };
            var result = await _projectController.Post(newProj, 1) as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var getProject = _projectController.Get() as OkObjectResult;
            var projects = getProject.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual(_refFixture.Projects.Count + 1, projects.Length);
            var getProjectById = _projectController.GetById("3") as OkObjectResult;
            var project = getProjectById.Value as ManagemAntsServer.Dbo.Project[];
            Assert.AreEqual(newProj.Name, project[0].Name);
        }

        /*        [Test]
                public async System.Threading.Tasks.Task Put()
                {
                    var bisProj = new ManagemAntsServer.Dbo.Project()
                    {
                        Id = 1,
                        Name = "Premier projet bis",
                        Description = "La description du premier projet bis"
                    };
                    var result = await _projectController.Put(bisProj) as OkObjectResult;
                    Assert.AreEqual(200, result.StatusCode);
                    var getProjectById = _projectController.GetById("1") as OkObjectResult;
                    var project = getProjectById.Value as ManagemAntsServer.Dbo.Project[];
                    Assert.AreEqual(bisProj.Name, project[0].Name);
                }*/

        [Test]
        public async System.Threading.Tasks.Task GetProjectByUserId()
        {
            var result = await _projectController.GetProjectByUserId("1", "_nofilter_") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as List<ManagemAntsServer.Dbo.Project>;
            Assert.AreEqual(2, projects.Count);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectCollaborators()
        {
            var result = await _projectController.GetProjectCollaborators("1") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as List<ManagemAntsServer.Dbo.UserWithRole>;
            Assert.AreEqual(2, projects.Count);
        }

        [Test]
        public async System.Threading.Tasks.Task GetProjectCollaboratorsByRole()
        {
            var result = await _projectController.GetProjectCollaboratorsByRole("1", "0") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as List<ManagemAntsServer.Dbo.User>;
            Assert.AreEqual(1, projects.Count);
        }

        [Test]
        public async System.Threading.Tasks.Task Post_ProjectHasUser()
        {
            var newProjUser = new ManagemAntsServer.Dbo.ProjectsHasUser()
            {
                Id = 4,
                ProjectId = 2,
                UserId = 2,
                Role = 0,
                Project = new ManagemAntsServer.Dbo.Project() { Id = 2, Name = "Deuxieme projet" },
                User = new ManagemAntsServer.Dbo.User() { Id = 2 }
            };
            var result = _projectController.Post(newProjUser) as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var getProjects = await _projectController.GetProjectByUserId("2", "_nofilter_") as OkObjectResult;
            var projects = getProjects.Value as List<ManagemAntsServer.Dbo.Project>;
            Assert.AreEqual(2, projects.Count);
        }

        /*        [Test]
                public async System.Threading.Tasks.Task RemoveUserFromProject()
                {
                    var newProjUser = new ManagemAntsServer.Dbo.ProjectsHasUser()
                    {
                        Id = 4,
                        ProjectId = 2,
                        UserId = 2,
                        Role = 0,
                        Project = new ManagemAntsServer.Dbo.Project() { Id = 2, Name = "Deuxieme projet" },
                        User = new ManagemAntsServer.Dbo.User() { Id = 2 }
                    };
                     _projectController.Post(newProjUser);
                    var result = await _projectController.RemoveUserFromProject("2", "2") as OkObjectResult;
                    Assert.AreEqual(200, result.StatusCode);
                    var getProjects = await _projectController.GetProjectByUserId("2", "_nofilter_") as OkObjectResult;
                    var projects = getProjects.Value as List<ManagemAntsServer.Dbo.Project>;
                    Assert.AreEqual(1, projects.Count);
               }*/

        /*        [Test]
                public async System.Threading.Tasks.Task PostMultipleUsers()
                {
                    var result = await _projectController.PostMultipleUsers("2", new string[] { "2" }) as OkObjectResult;
                    Assert.AreEqual(200, result.StatusCode);
                    var getProjects = await _projectController.GetProjectByUserId("2", "_nofilter_") as OkObjectResult;
                    var projects = getProjects.Value as List<ManagemAntsServer.Dbo.Project>;
                    Assert.AreEqual(2, projects.Count);
                }*/

        [Test]
        public void GetByFilter()
        {
            var result = _projectController.GetByFilter("1", "_nofilter_") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var projects = result.Value as List<ManagemAntsServer.Dbo.User>;
            Assert.AreEqual(2, projects.Count);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTasksFromUser()
        {
            var result = await _projectController.GetTasksFromUser("1") as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            var tasks = result.Value as List<ManagemAntsServer.Dbo.Task>;
            Assert.AreEqual(2, tasks.Count);
        }
    }
}
