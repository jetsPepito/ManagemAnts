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
        public async Task<IActionResult> Get()
        {
            var x = await _projectsHasUserRepository.Get();
            return Ok(x);
        }
    }
}
