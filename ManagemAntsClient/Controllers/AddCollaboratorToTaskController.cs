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
        private static Models.AddCollaboratorToTaskPage _page;

        public async Task<ActionResult> Index(string projectId, string taskId, string taskName, string userId, string searchFilter = "")
        {
            var user = this.GetLoggedUser();

            var searchUsers = new List<Models.User>();
            var client = Utils.Client.SetUpClient("Project/" + projectId + "/users/research/" + searchFilter);
            var response = await Utils.Client.GetAsync(client, "");
            if (response.IsSuccessStatusCode)
                searchUsers = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());

            var collaborators = await GetTaskCollaborators(taskId);

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
        public async Task<ActionResult> AddNewCollaboratorToTask(long collaboratorId)
        {
            HttpClient client = Utils.Client.SetUpClient("Task/Users");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(new
                {
                    taskId = long.Parse(_page.TaskId),
                    userId = collaboratorId
                })
            };
            var response = await Utils.Client.SendAsync(client, postRequest);
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
            HttpClient client = Utils.Client.SetUpClient("Task/" + _page.TaskId + "/User/" + collaboratorId);
            var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
            var response = await Utils.Client.SendAsync(client, deleteRequest);
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

        public static async Task<List<Models.User>> GetTaskCollaborators(string taskId)
        {
            var client = Utils.Client.SetUpClient("Task/" + taskId + "/users");
            HttpResponseMessage response = await Utils.Client.GetAsync(client, "");

            var collaborators = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                collaborators = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return collaborators;
        }

    }
}
