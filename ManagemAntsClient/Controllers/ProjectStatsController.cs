using ManagemAntsClient.Models;
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
    public class ProjectStatsController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private string url = "https://localhost:44352/api/";
        private static ProjectPage _projectPage;

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
            var filter = query.Get("filter");
            var myTask = query.Get("myTasks") == null ? "false" : query.Get("myTasks");
            var projectId = GetProjectId();
            var loggedUser = getLoggedUser();
            var tasks = (await GetTaskByProjectId(projectId, filter));
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


            _projectPage = new ProjectPage()
            {
                Project = project,
                LoggedUser = loggedUser,
                Tasks = tasks,
                Collaborators = collaborators,
                Mangers = managers,
                Creators = creators,
                isMyTasks = bool.Parse(myTask)
            };

            return View(_projectPage);
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
            var collaborators = _projectPage.Creators.Concat(_projectPage.Mangers.Concat(_projectPage.Collaborators)).ToList();
            var collaboratorsLabels = collaborators.Select(x => x.pseudo);
            var timeSpentTodoCollaborators = Enumerable.Repeat(0, collaborators.Count()).ToList();
            var timeSpentDoneCollaborators = Enumerable.Repeat(0, collaborators.Count()).ToList();
            var nbTodoCollaborators = Enumerable.Repeat(0, collaborators.Count()).ToList();
            var nbDoneCollaborators = Enumerable.Repeat(0, collaborators.Count()).ToList();

            var tasksTodo = new List<Models.Task>();
            var tasksInProgress = new List<Models.Task>();
            var tasksDone = new List<Models.Task>();
            var tasksDelivered = new List<Models.Task>();

            foreach (var task in _projectPage.Tasks)
            {
                switch (task.state)
                {
                    case 0:
                        tasksTodo.Add(task);
                        break;
                    case 1:
                        tasksInProgress.Add(task);
                        break;
                    case 2:
                        tasksDone.Add(task);
                        break;
                    case 3:
                        tasksDelivered.Add(task);
                        break;
                }
                foreach (var taskCollaborator in task.collaborators)
                {
                    var index = collaborators.FindIndex(x => x.id == taskCollaborator.id);
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
                nbTasksByState = new List<int> { tasksTodo.Count, tasksInProgress.Count, tasksDone.Count, tasksDelivered.Count },
                timeSpentTodoCollaborators = timeSpentTodoCollaborators,
                timeSpentDoneCollaborators = timeSpentDoneCollaborators,
                nbTodoCollaborators = nbTodoCollaborators,
                nbDoneCollaborators = nbDoneCollaborators,
                collaboratorsLabels = collaboratorsLabels
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
