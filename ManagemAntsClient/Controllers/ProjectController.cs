using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ManagemAntsClient.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private string url = "https://localhost:44352/api/";
        private static ProjectPage _projectPage;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        private HttpClient SetUpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private string GetProjectId()
        {
            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get("projectId");
        }

        private string GetTaskOpened()
        {
            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get("taskId");
        }

        public async Task<IActionResult> Index()
        {
            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            var filter = query.Get("filter");
            var myTask = query.Get("myTasks") == null ? "false" : query.Get("myTasks");
            var projectId = GetProjectId();
            var taskOpened = GetTaskOpened();
            if (taskOpened == null)
                taskOpened = "-1";
            var loggedUser = await getLoggedUser("5");
            var tasks = (await GetTaskByProjectId(projectId, filter));
            tasks = tasks.Where(x => !bool.Parse(myTask) || x.collaborators.Any(y => y.id == loggedUser.id)).ToList();
            var project = (await GetProjectById(projectId));
            tasks.Reverse();

            var collaborators = await GetCollaboratorsByRole(projectId, 2);
            var managers = await GetCollaboratorsByRole(projectId, 1);
            var creators = await GetCollaboratorsByRole(projectId, 0);

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
                    OpenedTask = long.Parse(taskOpened),
                    isMyTasks = bool.Parse(myTask)
                };

            return View(_projectPage);
        }

        [HttpGet]
        public async Task<Models.User> getLoggedUser(string userId)
        {
            var client = SetUpClient("User/" + userId);
            HttpResponseMessage response = client.GetAsync("").Result;
            var user = new Models.User();
            var responseUser = new List<Models.User>();

            if (response.IsSuccessStatusCode)
            {
                responseUser = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
                if (responseUser.Count == 0)
                {
                    //FIXME
                    throw new NotImplementedException();
                }
                else
                    user = responseUser[0];
            }

            return user;
        }

        public async Task<List<Models.User>> GetCollaboratorsByRole(string projectId, int roleValue)
        {
            var client = SetUpClient("project/" + projectId + "/users/role/" + roleValue);
            HttpResponseMessage response = client.GetAsync("").Result;

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }

        public async Task<List<Models.Task>> GetTaskByProjectId(string id, string filter)
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
            var client = SetUpClient("task/" + id + "?filter=" + filterVal);
            HttpResponseMessage response = client.GetAsync("").Result;
            var tasks = new List<Models.Task>();
            if (response.IsSuccessStatusCode)
            {
                tasks = await JsonSerializer.DeserializeAsync<List<Models.Task>>(await response.Content.ReadAsStreamAsync());
            }
            return tasks;
        }


        public async Task<Models.Project> GetProjectById(string id)
        {
            var client = SetUpClient("Project/" + id);
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

            var client = SetUpClient("task/");

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
            var client = SetUpClient("task/");

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
            var client = SetUpClient("task/" + taskId);
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

        public async Task<IActionResult> UpdateTask(Models.Task task)
        {
            var client = SetUpClient("task/");

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
            var client = SetUpClient("task/");
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
            var client = SetUpClient("project/" + _projectPage.Project.id);
            HttpResponseMessage response = await client.DeleteAsync("");

            return RedirectToAction("Index", "Dashboard");
        }

    }
}
