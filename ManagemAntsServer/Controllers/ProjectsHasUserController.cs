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
    public class ProjectsHasUserController : ControllerBase
    {
        private readonly IProjectsHasUserRepository _projectsHasUserRepository;

        public ProjectsHasUserController(IProjectsHasUserRepository projectsHasUserRepository)
        {
            _projectsHasUserRepository = projectsHasUserRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var x =  _projectsHasUserRepository.GetAll();
            return Ok(x);
        }

        [HttpGet("/api/[controller]/user/{userId}")]
        public IActionResult GetProjectByUserId(string userId)
        {
            var x = _projectsHasUserRepository.GetProjectByUserId(long.Parse(userId));
            return Ok(x);
        }


        [HttpPost("")]
        public async Task<IActionResult> Post(Dbo.ProjectsHasUser projectsHasUser)
        {
            var result = await _projectsHasUserRepository.Insert(projectsHasUser);

            return Ok(result);
        }


        [HttpPost("/api/[controller]/project/{projectId}")]
        public async Task<IActionResult> PostMultipleUsers(string projectId, string[] userIds)
        {
            var results = new List<Dbo.ProjectsHasUser>();
            foreach(var userId in userIds)
            {
                var newProjectHasUser = new Dbo.ProjectsHasUser();

                newProjectHasUser.ProjectId = long.Parse(projectId);
                newProjectHasUser.UserId = long.Parse(userId);
                // default role (2 -> collaborateur)
                newProjectHasUser.Role = 2;

                results.Add( await _projectsHasUserRepository.Insert(newProjectHasUser));
            }

            

            return Ok(results);
        }
    }
}
