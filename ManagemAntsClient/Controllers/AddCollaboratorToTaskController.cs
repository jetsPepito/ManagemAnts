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
    public class AddCollaboratorToTaskController : Controller
    {
        private string url = "https://localhost:44352/api/";
        private static Models.AddCollaboratorToTaskPage _page;
        private HttpClient SetUpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // GET: AddCollaboratorToTaskController
        public async Task<ActionResult> Index(string projectId, string taskId, string taskName, string userId, string searchFilter = "")
        {
            var user = getLoggedUser();

            var searchUsers = new List<Models.User>();
            var client = SetUpClient("Project/" + projectId + "/users/research/" + searchFilter);
            var response = client.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
                searchUsers = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            var collaborators = await GetCollaborators(taskId);

            searchUsers = searchUsers.Where(el => !collaborators.Any(collab => collab.pseudo == el.pseudo)).ToList();
            _page = new Models.AddCollaboratorToTaskPage()
            {
                ProjectId = projectId,
                TaskId = taskId,
                TaskName = taskName,
                LoggedUser = user,
                SearchCollaborators = searchUsers,
                Collaborators = collaborators,
                search = searchFilter,
                noResult = searchUsers.Count == 0 && searchFilter != ""
            };

            return View(_page);
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

        [HttpGet]
        public async Task<List<Models.User>> GetCollaborators(string taskId)
        {
            var client = SetUpClient("Task/" + taskId + "/Users");
            HttpResponseMessage response = client.GetAsync("").Result;

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }


        public ActionResult Research(string search)
        {
            return RedirectToAction("Index", "AddCollaboratorToTask", new
            {
                projectId = _page.ProjectId,
                taskId = _page.TaskId,
                taskName = _page.TaskName,
                userId = _page.LoggedUser.id.ToString(),
                searchFilter = search
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddNew(long collaboratorId)
        {
            HttpClient client = SetUpClient("Task/Users");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(new
                {
                    taskId = long.Parse(_page.TaskId),
                    userId = collaboratorId
                })
            };
            var response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "AddCollaboratorToTask", new
            {
                projectId = _page.ProjectId,
                taskId = _page.TaskId,
                taskName = _page.TaskName,
                userId = _page.LoggedUser.id.ToString(),
                search = _page.search
            });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveCollaborator(long collaboratorId)
        {
            HttpClient client = SetUpClient("Task/" + _page.TaskId + "/User/" + collaboratorId);
            var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
            var response = await client.SendAsync(deleteRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "AddCollaboratorToTask", new
            {
                projectId = _page.ProjectId,
                taskId = _page.TaskId,
                taskName = _page.TaskName,
                userId = _page.LoggedUser.id.ToString(),
                search = _page.search
            });
        }

        // GET: AddCollaboratorToTaskController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AddCollaboratorToTaskController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddCollaboratorToTaskController/Create
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

        // GET: AddCollaboratorToTaskController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AddCollaboratorToTaskController/Edit/5
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

        // GET: AddCollaboratorToTaskController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AddCollaboratorToTaskController/Delete/5
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
