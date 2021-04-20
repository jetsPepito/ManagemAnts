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
    public class AddCollaboratorController : Controller
    {
        private string url = "https://localhost:44352/api/";
        private static Models.AddCollaboratorPage _page;
        private HttpClient SetUpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // GET: AddCollaboratorController
        public async Task<ActionResult> Index(string projectId, string projectName, string userId, string searchFilter = "")
        {
            var user = getLoggedUser();

            var searchUsers = new List<Models.User>();
            if (!string.IsNullOrEmpty(searchFilter))
            {
                var client = SetUpClient("User/research/" + searchFilter);
                var response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                    searchUsers = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }

            var collaborators = await GetCollaborators(projectId);

            user.role = collaborators.Where(x => x.id == user.id).FirstOrDefault().role;

            searchUsers = searchUsers.Where(el => !collaborators.Any(collab => collab.pseudo == el.pseudo)).ToList();
            _page = new Models.AddCollaboratorPage()
            {
                ProjectId = projectId,
                ProjectName = projectName,
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

        public async Task<ActionResult> ModifyRole(string userId, string roleValue)
        {
            var client = SetUpClient("project/" + _page.ProjectId + "/user/" + userId + "/role/" + roleValue);

            var putRequest = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);

            var response = await client.SendAsync(putRequest);

            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "AddCollaborator", new
            {
                projectId = _page.ProjectId,
                projectName = _page.ProjectName,
                userId = _page.LoggedUser.id.ToString(),
                searchFilter = _page.search
            });
        }

        [HttpGet]
        public async Task<List<Models.User>> GetCollaborators(string projectId)
        {
            var client = SetUpClient("project/" + projectId + "/users");
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
            return RedirectToAction("Index", "AddCollaborator", new
            {
                projectId = _page.ProjectId,
                projectName = _page.ProjectName,
                userId = _page.LoggedUser.id.ToString(),
                searchFilter = search
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddNew(long collaboratorId)
        {
            HttpClient client = SetUpClient("Project/User");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(new
                {
                    projectId = long.Parse(_page.ProjectId),
                    userId = collaboratorId,
                    role = 2
                })
            };
            var response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "AddCollaborator", new
            {
                projectId = _page.ProjectId,
                projectName = _page.ProjectName,
                userId = _page.LoggedUser.id.ToString(),
                search = _page.search
            });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveCollaborator(long collaboratorId)
        {
            HttpClient client = SetUpClient("Project/" + _page.ProjectId + "/User/" + collaboratorId);
            var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
            var response = await client.SendAsync(deleteRequest);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "AddCollaborator", new
            {
                projectId = _page.ProjectId,
                projectName = _page.ProjectName,
                userId = _page.LoggedUser.id.ToString(),
                search = _page.search
            });
        }

        // GET: AddCollaboratorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AddCollaboratorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddCollaboratorController/Create
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

        // GET: AddCollaboratorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AddCollaboratorController/Edit/5
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

        // GET: AddCollaboratorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AddCollaboratorController/Delete/5
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
