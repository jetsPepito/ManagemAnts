﻿using ManagemAntsServer.DataAccess.Interfaces;
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
        public async Task<IActionResult> Get()
        {
            var result = await _projectRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("/api/[controller]/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _projectRepository.GetByPredicate(x => x.Id == long.Parse(id));
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
    }
}
