using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public static DashboardPage _page;

        public async Task<ActionResult> Index(string searchFilter = "")
        {
            
            var user = this.GetLoggedUser();

            HttpClient client = Utils.Client.SetUpClient("Project/user/" + user.id + "/research/" + searchFilter);
            HttpResponseMessage response = await Utils.Client.GetAsync(client, "");

            var projects = new List<Project>();
            if (response.IsSuccessStatusCode)
                projects = await JsonSerializer.DeserializeAsync<List<Project>>(await response.Content.ReadAsStreamAsync());

            _page = new DashboardPage() {
                Projects = new Projects(projects),
                LoggedUser = user,
                search = searchFilter
            };
            await GetStatsTasksByProjects();
            return View(_page);
        }

        [HttpGet]
        public async Task<ActionResult> GetStatsTasksByProjects()
        {
            var client = Utils.Client.SetUpClient("Project/stats/tasks/" + _page.LoggedUser.id);
            HttpResponseMessage response = await Utils.Client.GetAsync(client, "");
            var tasks = new List<Models.Task>();

            var labels = new List<string>();
            var valuesCount = new List<int>();
            var valuesTime = new List<int>();
            var valuesTimeSpent = new List<int>();

            if (response.IsSuccessStatusCode)
            {
                tasks = await JsonSerializer.DeserializeAsync<List<Models.Task>>(await response.Content.ReadAsStreamAsync());
                var groupedByProjects = tasks.GroupBy(el => el.projectId).ToList();
                foreach (var project in _page.Projects.projects)
                {
                    labels.Add(project.name);
                    int count = 0;
                    int time = 0;
                    int timeSpent = 0;
                    int index = groupedByProjects.FindIndex(x => x.Key == project.id);
                    if (index != -1)
                    {
                        count = groupedByProjects[index].Count();
                        foreach (var task in groupedByProjects[index])
                        {
                            if (task.state < 2)
                            {
                                time += task.duration;
                            }
                            if (task.state == 3)
                                timeSpent += task.timeSpent != null ? task.timeSpent.Value : 0;
                        }
                    }
                    valuesCount.Add(count);
                    valuesTime.Add(time);
                    valuesTimeSpent.Add(timeSpent);
                }
            }

            return Ok(new { labels = labels, valuesCount = valuesCount, valuesTime = valuesTime, valuesTimeSpent = valuesTimeSpent});
        }

        public ActionResult Research(string search)
        {
            return RedirectToAction("Index", "Dashboard", new
            {
                searchFilter = search
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddProject(string name, string description, string ownerId)
        {
            HttpClient client = Utils.Client.SetUpClient("Project?Owner=" + ownerId);
            var project = new Project() { name=name, description=description };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(project)
            };
            var response = await Utils.Client.SendAsync(client, postRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
