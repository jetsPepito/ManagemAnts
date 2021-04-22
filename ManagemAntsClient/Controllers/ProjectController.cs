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

            var tasks = (await Utils.CommunGet.GetTaskByProjectId(parameters["projectId"], "", parameters["myTask"], this.UserId()));

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
                isMyTasks = bool.Parse(parameters["myTask"])
                };

            return View(_projectPage);
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

            var responce = await Utils.Client.SendAsync(client, postRequest);

            responce.EnsureSuccessStatusCode();
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

            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = task.projectId });
        }

        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var client = Utils.Client.SetUpClient("task/" + taskId);
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
            var client = Utils.Client.SetUpClient("task/");

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce = await Utils.Client.SendAsync(client, putRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = _projectPage.Project.id });
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

            var responce = await Utils.Client.SendAsync(client, putRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project", new { projectId = _projectPage.Project.id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject()
        {
            var client = Utils.Client.SetUpClient("project/" + _projectPage.Project.id);
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
