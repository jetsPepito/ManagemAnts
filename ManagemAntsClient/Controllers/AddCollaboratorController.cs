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
        private static Models.AddCollaboratorPage _page;


        public async Task<ActionResult> Index(string projectId, string projectName, string userId, string searchFilter = "")
        {
            var user = this.GetLoggedUser();

            var searchUsers = new List<Models.User>();
            if (!string.IsNullOrEmpty(searchFilter))
            {
                var client = Utils.Client.SetUpClient("User/research/" + searchFilter);
                var response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                    searchUsers = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }

            var collaborators = await GetProjectCollaborators(projectId);

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


        public async Task<ActionResult> ModifyRole(string userId, string roleValue)
        {
            var client = Utils.Client.SetUpClient("project/" + _page.ProjectId + "/user/" + userId + "/role/" + roleValue);

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
        public async Task<ActionResult> AddNewCollaborator(long collaboratorId)
        {
            HttpClient client = Utils.Client.SetUpClient("Project/User");
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
            HttpClient client = Utils.Client.SetUpClient("Project/" + _page.ProjectId + "/User/" + collaboratorId);
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


        public static async Task<List<Models.User>> GetProjectCollaborators(string projectId)
        {
            var client = Utils.Client.SetUpClient("Project/" + projectId + "/users");
            HttpResponseMessage response = client.GetAsync("").Result;

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }

    }
}
