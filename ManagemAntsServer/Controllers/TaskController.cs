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
    public class TaskController: ControllerBase
    {

        private readonly ITaskRepository _taskrepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskrepository = taskRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var res = _taskrepository.GetAll();
            return Ok(res);
        }

        [HttpGet("/api/[controller]/{projectId}")]
        public IActionResult GetTaskByProjectId(string projectId, int filter)
        {
            var res = _taskrepository.GetByPredicate(x => x.ProjectId == long.Parse(projectId) && (filter == -1 || x.State == filter)) ;
            return Ok(res);
        }

        [HttpPost]
        public IActionResult Post(Dbo.Task task)
        {
            var res = _taskrepository.Insert(task);
            return Ok(res);
        }

        [HttpPut]
        public IActionResult Put(Dbo.Task task)
        {
            var res = _taskrepository.Update(task);
            return Ok(res);
        }

    }
}
