using AutoMapper;
using ManagemAntsServer.Controllers;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsTest.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagemAntsTest.API
{
    public class TaskControllerTest : BaseSetup
    {
        private TaskController _taskController;
        private LoginController _loginController;

        [SetUp]
        public void Setup()
        {
            SetupDb();
            var myProfile = new AutomapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            TaskRepository taskRepository = new TaskRepository(contextMock.Object, null, mapper);
            UsersHasTaskRepository usersHasTaskRepository = new UsersHasTaskRepository(contextMock.Object, null, mapper);
            _taskController = new TaskController(taskRepository, usersHasTaskRepository);

            UserRepository userRepository = new UserRepository(contextMock.Object, null, mapper);
            _loginController = new LoginController(userRepository);
        }

        [Test]
        public void Get()
        {
            var result = _taskController.Get() as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(_refFixture.Tasks.Count, tasks.Length);
        }

        [Test]
        public async Task GetTaskByProjectId()
        {
            var project = _refFixture.Projects[0];
            int filter = -1;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var tasksToGet = _refFixture.Tasks
                .Where(x => x.ProjectId == project.Id && (filter == -1 || x.State == filter))
                .ToList();

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(tasksToGet.Count, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsTrue(TaskUtils.IsEqualTasks(tasksToGet[i], tasks[i]));
            }
        }

        [Test]
        public async Task GetTaskByProjectIdFilter0()
        {
            var project = _refFixture.Projects[0];
            int filter = 0;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var tasksToGet = _refFixture.Tasks
                .Where(x => x.ProjectId == project.Id && (filter == -1 || x.State == filter))
                .ToList();

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(tasksToGet.Count, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsTrue(TaskUtils.IsEqualTasks(tasksToGet[i], tasks[i]));
            }
        }

        [Test]
        public async Task GetTaskByProjectIdFilter1()
        {
            var project = _refFixture.Projects[0];
            int filter = 1;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var tasksToGet = _refFixture.Tasks
                .Where(x => x.ProjectId == project.Id && (filter == -1 || x.State == filter))
                .ToList();

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(tasksToGet.Count, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsTrue(TaskUtils.IsEqualTasks(tasksToGet[i], tasks[i]));
            }
        }

        [Test]
        public async Task GetTaskByProjectIdFilter2()
        {
            var project = _refFixture.Projects[0];
            int filter = 2;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var tasksToGet = _refFixture.Tasks
                .Where(x => x.ProjectId == project.Id && (filter == -1 || x.State == filter))
                .ToList();

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(tasksToGet.Count, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsTrue(TaskUtils.IsEqualTasks(tasksToGet[i], tasks[i]));
            }
        }

        [Test]
        public async Task GetTaskByProjectIdFilter3()
        {
            var project = _refFixture.Projects[0];
            int filter = 3;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var tasksToGet = _refFixture.Tasks
                .Where(x => x.ProjectId == project.Id && (filter == -1 || x.State == filter))
                .ToList();

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(tasksToGet.Count, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsTrue(TaskUtils.IsEqualTasks(tasksToGet[i], tasks[i]));
            }
        }

        [Test]
        public async Task GetTaskByProjectIdNotExistingFilter()
        {
            var project = _refFixture.Projects[0];
            int filter = 18;
            var result = await _taskController.GetTaskByProjectId(project.Id.ToString(), filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var tasks = result.Value as ManagemAntsServer.Dbo.Task[];
            Assert.AreEqual(0, tasks.Length);
        }

        [Test]
        public void Post()
        {
            var project = _refFixture.Projects[0];
            var newTask = new ManagemAntsServer.Dbo.Task()
            {
                Name = "NewTaskName",
                Description = "",
                CreatedAt = new DateTime(),
                Duration = 2,
                State = 0,
                ProjectId = project.Id,
            };

            var result = _taskController.Post(newTask) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var task = (result.Value as Task<ManagemAntsServer.Dbo.Task>).Result;
            Assert.IsTrue(TaskUtils.IsEqualTasks(newTask, task));
        }

        /*[Test]
        public void Put()
        {
            var project = _refFixture.Projects[0];
            var firstTask = _refFixture.Tasks[0];
            var taskToModify = new ManagemAntsServer.Dbo.Task()
            {
                Id = firstTask.Id,
                Name = "NewTaskNameUpdate",
                Description = "NewTaskDescription",
                CreatedAt = new DateTime(),
                Duration = 1,
                State = 1,
                ProjectId = firstTask.ProjectId,
                TimeSpent = null,
            };

            var result = _taskController.Put(taskToModify) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var task = (result.Value as Task<ManagemAntsServer.Dbo.Task>).Result;
            Assert.IsTrue(TaskUtils.IsEqualTasks(taskToModify, task));
            Assert.IsFalse(TaskUtils.IsEqualTasks(firstTask, task));
        }*/

        [Test]
        public async Task GetTaskCollaborators()
        {
            var task = _refFixture.Tasks[0];
            var result = await _taskController.GetTaskCollaborators(task.Id.ToString()) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var userHasTasks = _refFixture.UsersHasTasks.Where(x => x.TaskId == task.Id).ToList();

            var collaborators = result.Value as List<ManagemAntsServer.Dbo.User>;
            Assert.AreEqual(userHasTasks.Count, collaborators.Count);
        }

        [Test]
        public async Task AddTaskCollaborators()
        {
            var newUser = new ManagemAntsServer.Dbo.User()
            {
                Pseudo = "ThisIsANewPseudoTask",
                Firstname = "ThisIsANewFirstanameTask",
                Lastname = "ThisIsANewLastnameTask",
                Password = "testtestTask"
            };
            var resultUser = await _loginController.SignUp(newUser) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var user = (resultUser.Value as List<ManagemAntsServer.Dbo.User>).FirstOrDefault();
            var task = _refFixture.Tasks[0];

            var newUserHasTask = new ManagemAntsServer.Dbo.UsersHasTask()
            {
                UserId = user.Id,
                TaskId = task.Id
            };

            var result = await _taskController.AddTaskCollaborators(newUserHasTask) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var userHasTask = result.Value as ManagemAntsServer.Dbo.UsersHasTask;
            Assert.IsTrue(TaskUtils.IsEqualUserHasTasks(newUserHasTask, userHasTask));
        }

        [Test]
        public async Task DeleteTaskCollaborator()
        {
            var newUser = new ManagemAntsServer.Dbo.User()
            {
                Pseudo = "ThisIsANewPseudoTaskToDelete",
                Firstname = "ThisIsANewFirstanameTaskToDelete",
                Lastname = "ThisIsANewLastnameTaskToDelete",
                Password = "testtestTaskToDelete"
            };
            var resultUser = await _loginController.SignUp(newUser) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var user = (resultUser.Value as List<ManagemAntsServer.Dbo.User>).FirstOrDefault();
            var task = _refFixture.Tasks[0];

            var newUserHasTask = new ManagemAntsServer.Dbo.UsersHasTask()
            {
                UserId = user.Id,
                TaskId = task.Id
            };

            var resultUserHasTask = await _taskController.AddTaskCollaborators(newUserHasTask) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var userHasTask = resultUserHasTask.Value as ManagemAntsServer.Dbo.UsersHasTask;

            var result = await _taskController.DeleteTaskCollaborator(userHasTask.TaskId.ToString(), userHasTask.UserId.ToString()) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value as bool?);
        }

        /*[Test]
        public async Task RemoveTaskFromProject()
        {
            var project = _refFixture.Projects[0];
            var newTask = new ManagemAntsServer.Dbo.Task()
            {
                Name = "TaskToDelete",
                Description = "",
                CreatedAt = new DateTime(),
                Duration = 2,
                State = 0,
                ProjectId = project.Id,
            };

            var resultTask = _taskController.Post(newTask) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            var task = (resultTask.Value as Task<ManagemAntsServer.Dbo.Task>).Result;

            var result = await _taskController.RemoveTaskFromProject(task.Id.ToString()) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value as bool?);
        }*/
    }
}
