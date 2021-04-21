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
        private static ProjectStatsPage _page;

        public ProjectStatsController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        private  Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>();

            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);

            parameters.Add("querySelectedCollaborator", query.Get("selectedCollaborator"));

            var myTaskTmp = query.Get("myTasks");
            parameters.Add("myTask", myTaskTmp == null ? "false" : query.Get("myTasks"));

            parameters.Add("projectId", query.Get("projectId"));

            return parameters;
        }

        public async Task<IActionResult> Index()
        {

            var parameters = GetParameters();

            Models.User selectedCollaborator = null;
            var isInCollaboratorTab = false;

            
            if (!string.IsNullOrEmpty(parameters["querySelectedCollaborator"]))
            {
                isInCollaboratorTab = true;
                selectedCollaborator = await getUserByPseudo(parameters["querySelectedCollaborator"].Split(' ')[0]);
            }

            var loggedUser = this.GetLoggedUser();

            var tasks = (await Utils.CommunGet.GetTaskByProjectId(parameters["projectId"], "", parameters["myTask"], this.UserId()));
            
            var project = (await Utils.CommunGet.GetProjectById(parameters["projectId"]));

            var collaborators = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 2);
            var managers = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 1);
            var creators = await Utils.CommunGet.GetCollaboratorsByRole(parameters["projectId"], 0);

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
            var client = Utils.Client.SetUpClient("user/pseudo/" + pseudo);
            HttpResponseMessage response = client.GetAsync("").Result;

            var user = new List<Models.User>();
            if (response.IsSuccessStatusCode)
            {
                user = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
            }
            return user.FirstOrDefault();
        }

        

        [HttpGet]
        public async Task<ActionResult> GetProjectStats()
        {
            var collaboratorsLabels = _page.AllCollaborators.Select(x => x.pseudo).ToList();
            collaboratorsLabels.Insert(0, "Sans collaborateur");
            var timeSpentTodoCollaborators = Enumerable.Repeat(0, collaboratorsLabels.Count()).ToList();
            var timeSpentDoneCollaborators = Enumerable.Repeat(0, collaboratorsLabels.Count()).ToList();
            var nbTodoCollaborators = Enumerable.Repeat(0, collaboratorsLabels.Count()).ToList();
            var nbDoneCollaborators = Enumerable.Repeat(0, collaboratorsLabels.Count()).ToList();

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
                            timeSpentTodoCollaborators[index + 1] += task.duration;
                            nbTodoCollaborators[index + 1] += 1;
                        }
                        else
                        {
                            timeSpentDoneCollaborators[index + 1] += task.timeSpent ?? 0;
                            nbDoneCollaborators[index + 1] += 1;
                        }
                    }
                }

                if (task.collaborators.Count == 0)
                {
                    if (task.state < 2)
                    {
                        timeSpentTodoCollaborators[0] += task.duration;
                        nbTodoCollaborators[0] += 1;
                    }
                    else
                    {
                        timeSpentDoneCollaborators[0] += task.timeSpent ?? 0;
                        nbDoneCollaborators[0] += 1;
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

       
    }
}
