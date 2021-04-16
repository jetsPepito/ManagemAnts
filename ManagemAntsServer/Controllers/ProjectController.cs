using ManagemAntsServer.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectsHasUserRepository _projectsHasUserRepository;
        private readonly ITaskRepository _taskrepository;
        private readonly IUsersHasTaskRepository _usersHasTaskRepository;

        public ProjectController(IProjectRepository projectRepository, IProjectsHasUserRepository projectsHasUserRepository, ITaskRepository taskrepository, IUsersHasTaskRepository usersHasTaskRepository)
        {
            _projectRepository = projectRepository;
            _projectsHasUserRepository = projectsHasUserRepository;
            _taskrepository = taskrepository;
            _usersHasTaskRepository = usersHasTaskRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var result =  _projectRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("/api/[controller]/{id}")]
        public IActionResult GetById(string id)
        {
            var result = _projectRepository.GetByPredicate(x => x.Id == long.Parse(id));
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(Dbo.Project project, long Owner)
        {
            var result = await _projectRepository.Insert(project);

            var newProjectHasUser = new Dbo.ProjectsHasUser
            {
                UserId = Owner,
                ProjectId = result.Id,
                Role = 0,
            };

            await _projectsHasUserRepository.Insert(newProjectHasUser);

            return Ok(result);
        }

        [HttpGet("/api/[controller]/user/{userId}")]
        public async Task<IActionResult> GetProjectByUserId(string userId)
        {
            var x = await _projectsHasUserRepository.GetProjectByUserId(long.Parse(userId));
            return Ok(x);
        }

        [HttpGet("/api/[controller]/{projectId}/users")]
        public async Task<IActionResult> GetProjectCollaborators(string projectId)
        {
            var x = await _projectsHasUserRepository.GetProjectCollaborators(long.Parse(projectId));
            return Ok(x);
        }

        [HttpGet("/api/[controller]/{projectId}/users/role/{roleValue}")]
        public async Task<IActionResult> GetProjectCollaboratorsByRole(string projectId, string roleValue)
        {
            var x = await _projectsHasUserRepository.GetProjectCollaboratorsByRole(long.Parse(projectId), int.Parse(roleValue));
            return Ok(x);
        }

        [HttpPut("/api/[controller]/{projectId}/user/{userId}/role/{roleValue}")]
        public async Task<IActionResult> updateProjectHasUser(string projectId, string userId, string roleValue)
        {
            Dbo.ProjectsHasUser projectHasUser = _projectsHasUserRepository.GetByPredicate(
                x => x.ProjectId == long.Parse(projectId) && x.UserId == long.Parse(userId)).FirstOrDefault();
            projectHasUser.Role = int.Parse(roleValue);
            var res = await _projectsHasUserRepository.Update(projectHasUser);
            return Ok(res);
        }

        [HttpPost("/api/[controller]/user")]
        public IActionResult Post(Dbo.ProjectsHasUser projectsHasUser)
        {
            var result = _projectsHasUserRepository.Insert(projectsHasUser);

            return Ok(result);
        }


        [HttpDelete("/api/[controller]/{projectId}/user/{userId}")]
        public async Task<IActionResult> RemoveUserFromProject(string projectId, string userId)
        {
            var result = await _projectsHasUserRepository.removeUserFromProject(long.Parse(projectId), long.Parse(userId));

            var res = _taskrepository.GetByPredicate(x => x.ProjectId == long.Parse(projectId)).Select(x => x.Id).ToList();

            result = result && await _usersHasTaskRepository.removeUserFromTasks(res, long.Parse(userId));

            return Ok(result);
        }


        [HttpPost("/api/[controller]/project/{projectId}")]
        public async Task<IActionResult> PostMultipleUsers(string projectId, string[] userIds)
        {
            var results = new List<Dbo.ProjectsHasUser>();
            foreach (var userId in userIds)
            {
                var newProjectHasUser = new Dbo.ProjectsHasUser();

                newProjectHasUser.ProjectId = long.Parse(projectId);
                newProjectHasUser.UserId = long.Parse(userId);
                // default role (2 -> collaborateur)
                newProjectHasUser.Role = 2;

                results.Add(await _projectsHasUserRepository.Insert(newProjectHasUser));
            }

            return Ok(results);
        }

        [HttpGet("/api/[controller]/{projectId}/users/research/{filter=_nofilter_}")]
        public IActionResult GetByFilter(string projectId, string filter)
        {
            if (filter == "_nofilter_")
                filter = "";
            var result = _projectsHasUserRepository.GetProjectCollaboratorsByFilter(long.Parse(projectId), filter);
            return Ok(result);
        }
    }
}
