using ManagemAntsClient.Models;
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
            
            var user = await getLoggedUser("1");

            HttpClient client = SetupClient("Project/user/" + user.id + "/research/" + searchFilter);
            HttpResponseMessage response = client.GetAsync("").Result;

            IEnumerable<Project> projects = null;
            if (response.IsSuccessStatusCode)
                projects = await JsonSerializer.DeserializeAsync<IEnumerable<Project>>(await response.Content.ReadAsStreamAsync());

            _page = new DashboardPage() {
                Projects = new Projects(projects),
                LoggedUser = user,
            };
            return View(_page);
        }

        public ActionResult Research(string search)
        {
            return RedirectToAction("Index", "Dashboard", new
            {
                searchFilter = search
            });
        }


        [HttpGet]
        public async Task<Models.User> getLoggedUser(string userId)
        {
            var client = SetupClient("User/" + userId);
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
