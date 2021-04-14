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
        
        public ProjectController(IProjectRepository projectRepository, IProjectsHasUserRepository projectsHasUserRepository)
        {
            _projectRepository = projectRepository;
            _projectsHasUserRepository = projectsHasUserRepository;
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


        [HttpPost("/api/[controller]/user")]
        public async Task<IActionResult> Post(Dbo.ProjectsHasUser projectsHasUser)
        {
            var result = await _projectsHasUserRepository.Insert(projectsHasUser);

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
    }
}
