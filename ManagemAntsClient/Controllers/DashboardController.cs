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
        private string url = "https://localhost:44352/api/";

        private HttpClient SetupClient(string endpoint)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // GET: DashboardController
        public async Task<ActionResult> Index(string searchFilter = "")
        {
            
            var user = getLoggedUser();

            HttpClient client = SetupClient("Project/user/" + user.id + "/research/" + searchFilter);
            HttpResponseMessage response = client.GetAsync("").Result;

            IEnumerable<Project> projects = null;
            if (response.IsSuccessStatusCode)
                projects = await JsonSerializer.DeserializeAsync<IEnumerable<Project>>(await response.Content.ReadAsStreamAsync());

            _page = new DashboardPage() {
                Projects = new Projects(projects),
                LoggedUser = user,
            };
            await GetStatsTasksByProjects();
            return View(_page);
        }

        [HttpGet]
        public async Task<ActionResult> GetStatsTasksByProjects()
        {
            var client = SetupClient("Project/stats/tasks/" + _page.LoggedUser.id);
            HttpResponseMessage response = client.GetAsync("").Result;
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


        [HttpGet]
        public Models.User getLoggedUser()
        {

            var names = this.UserName().Split(' ');
            var firstname = names[0];
            var lastname = names[1];

            var user = new Models.User()
            {
                id = long.Parse(this.UserId()),
                firstname = firstname,
                lastname = lastname,
                pseudo = this.UserPseudo(),
            };

            return user;
        }

        [HttpPost]
        public async Task<ActionResult> AddProject(string name, string description, string ownerId)
        {
            HttpClient client = SetupClient("Project?Owner=" + ownerId);
            var project = new Project() { name=name, description=description };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(project)
            };
            var response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
