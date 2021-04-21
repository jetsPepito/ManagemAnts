using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ManagemAntsClient.Controllers
{
    [Authorize]
    public class ProjectStatsController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private string url = "https://localhost:44352/api/";
        private static ProjectStatsPage _page;

        public ProjectStatsController(ILogger<ProjectController> logger)
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

        public async Task<IActionResult> Index()
        {
            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            var querySelectedCollaborator = query.Get("selectedCollaborator");
            Models.User? selectedCollaborator = null;
            var isInCollaboratorTab = false;
            if (!string.IsNullOrEmpty(querySelectedCollaborator))
            {
                isInCollaboratorTab = true;
                selectedCollaborator = await getUserByPseudo(querySelectedCollaborator.Split(' ')[0]);
            }
            var myTask = query.Get("myTasks") == null ? "false" : query.Get("myTasks");
            var projectId = GetProjectId();
            var loggedUser = getLoggedUser();
            var tasks = (await GetTaskByProjectId(projectId, ""));
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


            _page = new ProjectStatsPage()
            {
                Project = project,
                LoggedUser = loggedUser,
                Tasks = tasks,
                Collaborators = collaborators,
                Managers = managers,
                Creators = creators,
                AllCollaborators = creators.Concat(managers.Concat(collaborators)).ToList(),
                SelectedCollaborator = selectedCollaborator,
                isInCollaboratorTab = isInCollaboratorTab,
            };

            return View(_page);
        }

        public async Task<Models.User> getUserByPseudo(string pseudo)
        {
            var client = SetUpClient("user/pseudo/" + pseudo);
            HttpResponseMessage response = client.GetAsync("").Result;

            var user = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                user = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return user.FirstOrDefault();
        }

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

        [HttpGet]
        public async Task<ActionResult> GetProjectStats()
        {
            var collaboratorsLabels = _page.AllCollaborators.Select(x => x.pseudo);
            var timeSpentTodoCollaborators = Enumerable.Repeat(0, _page.AllCollaborators.Count()).ToList();
            var timeSpentDoneCollaborators = Enumerable.Repeat(0, _page.AllCollaborators.Count()).ToList();
            var nbTodoCollaborators = Enumerable.Repeat(0, _page.AllCollaborators.Count()).ToList();
            var nbDoneCollaborators = Enumerable.Repeat(0, _page.AllCollaborators.Count()).ToList();

            var nbTasksByState = Enumerable.Repeat(0, 4).ToList();
            var timeByState = Enumerable.Repeat(0, 4).ToList();

            var collaboratorNbByState = Enumerable.Repeat(0, 4).ToList();
            var collaboratorTimeByState = Enumerable.Repeat(0, 4).ToList();
            var collaboratorDelay = Enumerable.Repeat(0, 2).ToList();

            var myStatsNbByState = Enumerable.Repeat(0, 4).ToList();
            var myStatsTimeByState = Enumerable.Repeat(0, 4).ToList();
            var myStatsDelay = Enumerable.Repeat(0, 2).ToList();

            foreach (var task in _page.Tasks)
            {
                nbTasksByState[task.state] += 1;
                timeByState[task.state] += task.duration;

                foreach (var taskCollaborator in task.collaborators)
                {
                    if (_page.SelectedCollaborator != null && taskCollaborator.id == _page.SelectedCollaborator.id)
                    {
                        collaboratorNbByState[task.state] += 1;
                        collaboratorTimeByState[task.state] += task.duration;
                        if (task.state > 2)
                        {
                            collaboratorDelay[0] += task.duration;
                            collaboratorDelay[1] += task.timeSpent ?? 0;
                        }
                    }
                    if (taskCollaborator.id == _page.LoggedUser.id)
                    {
                        myStatsNbByState[task.state] += 1;
                        myStatsTimeByState[task.state] += task.duration;
                        if (task.state > 2)
                        {
                            myStatsDelay[0] += task.duration;
                            myStatsDelay[1] += task.timeSpent ?? 0;
                        }
                    }

                    var index = _page.AllCollaborators.FindIndex(x => x.id == taskCollaborator.id);
                    if (index != -1)
                    {
                        if (task.state < 2)
                        {
                            timeSpentTodoCollaborators[index] += task.duration;
                            nbTodoCollaborators[index] += 1;
                        }
                        else
                        {
                            timeSpentDoneCollaborators[index] += task.timeSpent ?? 0;
                            nbDoneCollaborators[index] += 1;
                        }
                    }
                }
            }

            return Ok(new {
                nbTasksByState = nbTasksByState,
                timeByState = timeByState,
                timeSpentTodoCollaborators = timeSpentTodoCollaborators,
                timeSpentDoneCollaborators = timeSpentDoneCollaborators,
                nbTodoCollaborators = nbTodoCollaborators,
                nbDoneCollaborators = nbDoneCollaborators,
                collaboratorsLabels = collaboratorsLabels,
                collaboratorNbTasksByState = collaboratorNbByState,
                collaboratorTimeByState = collaboratorTimeByState,
                collaboratorDelay = collaboratorDelay,
                myStatsNbTasksByState = myStatsNbByState,
                myStatsTimeByState = myStatsTimeByState,
                myStatsDelay = myStatsDelay,
            });
        }

        // GET: ProjectStatsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProjectStatsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectStatsController/Create
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

        // GET: ProjectStatsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectStatsController/Edit/5
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

        // GET: ProjectStatsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectStatsController/Delete/5
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
