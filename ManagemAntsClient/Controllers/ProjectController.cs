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

        public async Task<IActionResult> Index()
        {

            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            var filter = query.Get("filter");
            var tasks = (await GetTaskByProjectId("2", filter));
            tasks.Reverse();

            return View(
                new ProjectPage() {
                    Project = new Project() { id = 1, name = "Ouistiti", description = "Un ptit singe tout mimi" },
                    LoggedUser = new User() { Pseudo = "Kaijo", Firstname = "Jeremie", Lastname = "Zeitoun" },
                    Tasks = tasks
                });
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
            HttpResponseMessage responce = client.GetAsync("").Result;
            var tasks = new List<Models.Task>();
            if (responce.IsSuccessStatusCode)
            {
                tasks = await JsonSerializer.DeserializeAsync<List<Models.Task>>(await responce.Content.ReadAsStreamAsync());
            }
            return tasks;
        }

        [HttpPost]
        public async Task<IActionResult> PostTaskAsync(string name, string desc, int duration, int state)
        {
            var client = SetUpClient("task/");

            var task = new Models.Task()
            {
                name = name,
                duration = duration,
                state = state,
                description = desc,
                createdAt = DateTime.Now,
                projectId = 2
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(task)
            };

            var responce =  await client.SendAsync(postRequest);

            responce.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Project");
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
            return RedirectToAction("Index", "Project");
        }
    }
}
