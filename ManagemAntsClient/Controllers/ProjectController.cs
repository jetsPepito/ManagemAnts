using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ManagemAntsClient.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private static ProjectPage _projectPage;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var parameters = GetParameters();

            var loggedUser = this.GetLoggedUser();

            var tasks = sortTasks(await Utils.CommunGet.GetTaskByProjectId(parameters["projectId"], parameters["filter"], parameters["myTasks"], this.UserId()));

            var project = (await Utils.CommunGet.GetProjectById(parameters["projectId"]));

            var collaborators = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 2);
            var managers = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 1);
            var creators = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 0);

            if (creators.Any(x => x.id == loggedUser.id))
                loggedUser.role = 0;
            else if (managers.Any(x => x.id == loggedUser.id))
                loggedUser.role = 1;

            _projectPage = new ProjectPage() {
                Project = project,
                LoggedUser = loggedUser,
                Tasks = tasks,
                Collaborators = collaborators,
                Mangers = managers,
                Creators = creators,
                OpenedTask = long.Parse(parameters["taskOpen"]),
                isMyTasks = bool.Parse(parameters["myTasks"])
                };

            return View(_projectPage);
        }

        private List<Models.Task> sortTasks(List<Models.Task> tasks)
        {
            return new List<List<Models.Task>>()
            {
                tasks.Where(x => x.state == 2).ToList(),
                tasks.Where(x => x.state == 1).ToList(),
                tasks.Where(x => x.state == 0).ToList(),
                tasks.Where(x => x.state == 3).ToList(),
            }.SelectMany(x => x).ToList();
        }


        [HttpPost]
        public async Task<IActionResult> PostTaskAsync(string name, string desc, int duration, int state, string projectId)
        {
            var task = new Models.Task()
            {
                name = name,
                duration = duration,
                state = state,
                description = desc,
                createdAt = DateTime.Now,
                projectId = int.Parse(projectId)
            };

            var client = Utils.Client.SetUpClient("task/");

            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var response = await Utils.Client.SendAsync(client, postRequest);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("ProjectController PostTaskAsync");
                errorWriter.WriteLine(e.Message);
            }
            return RedirectToAction("Index", "Project", new { projectId = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> PutTaskAsync(Models.Task task)
        {
            var client = Utils.Client.SetUpClient("task/");

            var postRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var response = await Utils.Client.SendAsync(client, postRequest);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("ProjectController PutTaskAsync");
                errorWriter.WriteLine(e.Message);
            }

            return RedirectToAction("Index", "Project", new
            {
                projectId = _projectPage.Project.id,
                myTasks = _projectPage.isMyTasks
            });
        }

        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var client = Utils.Client.SetUpClient("task/" + taskId);
            HttpResponseMessage response = await client.DeleteAsync("");

            return RedirectToAction("Index", "Project", new
            {
                projectId = _projectPage.Project.id,
                myTasks = _projectPage.isMyTasks
            });
        }

        [HttpPost]
        public async Task<IActionResult> NextStateTask(string taskId)
        {
            var tasks = _projectPage.Tasks.Where(el => el.id == long.Parse(taskId));
            if (tasks.Count() != 0)
            {
                var task = tasks.FirstOrDefault();
                task.state += 1;
                return await UpdateTask(task);
            }
            else
                return RedirectToAction("Index", "Project", new
                {
                    projectId = _projectPage.Project.id,
                    myTasks = _projectPage.isMyTasks
                });
        }

        [HttpPost]
        public async Task<IActionResult> BackStateTask(string taskId)
        {
            var tasks = _projectPage.Tasks.Where(el => el.id == long.Parse(taskId));
            if (tasks.Count() != 0)
            {
                var task = tasks.FirstOrDefault();
                task.state -= 1;
                return await UpdateTask(task);
            }
            else
                return RedirectToAction("Index", "Project", new
                {
                    projectId = _projectPage.Project.id,
                    myTasks = _projectPage.isMyTasks
                });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(Models.Task task)
        {
            var client = Utils.Client.SetUpClient("task/");

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce = await Utils.Client.SendAsync(client, putRequest);

            return RedirectToAction("Index", "Project", new {
                projectId = _projectPage.Project.id,
                myTasks = _projectPage.isMyTasks
            });
        }

        [HttpGet]
        public async Task<IActionResult> FinishTaskAsync(Models.Task task)
        {
            var client = Utils.Client.SetUpClient("task/");
            task.state += 1;

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var response = await Utils.Client.SendAsync(client, putRequest);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("ProjectController FinishTaskAsync");
                errorWriter.WriteLine(e.Message);
            }

            return RedirectToAction("Index", "Project", new
            {
                projectId = _projectPage.Project.id,
                myTasks = _projectPage.isMyTasks
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject()
        {
            var client = Utils.Client.SetUpClient("project/" + _projectPage.Project.id);
            HttpResponseMessage response = await client.DeleteAsync("");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProject(Project project)
        {
            var client = Utils.Client.SetUpClient("project/");

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(project)
            };

            var responce = await Utils.Client.SendAsync(client, putRequest);

            return RedirectToAction("Index", "Project", new
            {
                projectId = _projectPage.Project.id,
                myTasks = _projectPage.isMyTasks
            });
        }


        private Dictionary<string, string> GetParameters()
        {
            var res = new Dictionary<string, string>();

            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);

            var filterTmp = query.Get("filter");
            res.Add("filter", filterTmp == null ? "" : filterTmp);

            var myTaskTmp = query.Get("myTasks");
            res.Add("myTasks", myTaskTmp == null ? "false" : myTaskTmp);

            res.Add("projectId", query.Get("projectId"));

            var taskOpenTmp = query.Get("taskId");
            res.Add("taskOpen", taskOpenTmp == null ? "-1" : taskOpenTmp);

            return res;
        }
    }
}
