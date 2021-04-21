using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

            var tasks = (await GetTaskByProjectId(parameters["projectId"],
                    parameters["filter"],
                    bool.Parse(parameters["myTask"])));

            var project = (await GetProjectById(parameters["projectId"]));

            var collaborators = await GetCollaboratorsByRole(parameters["projectId"], 2);
            var managers = await GetCollaboratorsByRole(parameters["projectId"], 1);
            var creators = await GetCollaboratorsByRole(parameters["projectId"], 0);

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
                isMyTasks = bool.Parse(parameters["myTask"])
                };

            return View(_projectPage);
        }

        public async Task<List<Models.User>> GetCollaboratorsByRole(string projectId, int roleValue)
        {
            var client = this.SetUpClient("project/" + projectId + "/users/role/" + roleValue);
            HttpResponseMessage response = client.GetAsync("").Result;

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }

        public async Task<List<Models.Task>> GetTaskByProjectId(string id, string filter, bool myTask)
        {
            var filterVal = -1;
            switch (filter)
            {
                case "A faire":
                    filterVal = 0;
                    break;
                case "En cours":
                    filterVal = 1;
                    break;
                case "Fait":
                    filterVal = 2;
                    break;
                case "Rendu":
                    filterVal = 3;
                    break;
                default:
                    filterVal = -1;
                    break;
            }
            var client = this.SetUpClient("task/" + id + "?filter=" + filterVal);
            HttpResponseMessage response = client.GetAsync("").Result;
            var tasks = new List<Models.Task>();
            if (response.IsSuccessStatusCode)
            {
                tasks = await JsonSerializer.DeserializeAsync<List<Models.Task>>(await response.Content.ReadAsStreamAsync());
            }

            return tasks.Where(x => !myTask || x.collaborators.Any(y => y.id == this.UserId())).Reverse().ToList();
        }


        public async Task<Models.Project> GetProjectById(string id)
        {
            var client = this.SetUpClient("Project/" + id);
            HttpResponseMessage responce = client.GetAsync("").Result;
            var project = new List<Project>();
            if (responce.IsSuccessStatusCode)
            {
                var p = (await responce.Content.ReadAsStringAsync());
                project = await JsonSerializer.DeserializeAsync<List<Models.Project>>(await responce.Content.ReadAsStreamAsync());
            }
            return project[0];
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

            var client = this.SetUpClient("task/");

            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce =  await client.SendAsync(postRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> PutTaskAsync(Models.Task task)
        {
            var client = this.SetUpClient("task/");

            var postRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = task.projectId });
        }

        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var client = this.SetUpClient("task/" + taskId);
            HttpResponseMessage response = await client.DeleteAsync("");

            return RedirectToAction("Index", "Project", new { projectId = _projectPage.Project.id });
        }

        [HttpPost]
        public async Task<IActionResult> NextStateTask(Models.Task task)
        {
            task.state += 1;
            return await UpdateTask(task);
        }

        [HttpPost]
        public async Task<IActionResult> BackStateTask(Models.Task task)
        {
            task.state -= 1;
            return await UpdateTask(task);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(Models.Task task)
        {
            var client = this.SetUpClient("task/");

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce = await client.SendAsync(putRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = _projectPage.Project.id });
        }

        [HttpGet]
        public async Task<IActionResult> FinishTaskAsync(Models.Task task)
        {
            var client = this.SetUpClient("task/");
            task.state += 1;

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce = await client.SendAsync(putRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = _projectPage.Project.id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject()
        {
            var client = this.SetUpClient("project/" + _projectPage.Project.id);
            HttpResponseMessage response = await client.DeleteAsync("");

            return RedirectToAction("Index", "Dashboard");
        }


        private Dictionary<string, string> GetParameters()
        {
            var res = new Dictionary<string, string>();

            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);

            res.Add("filter", query.Get("filter"));

            var myTaskTmp = query.Get("myTasks");
            res.Add("myTask", myTaskTmp == null ? "false" : myTaskTmp);

            res.Add("projectId", query.Get("projectId"));

            var taskOpenTmp = query.Get("taskId");
            res.Add("taskOpen", taskOpenTmp == null ? "-1" : taskOpenTmp);

            return res;
        }

    }
}
