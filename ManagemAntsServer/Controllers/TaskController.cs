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
        private readonly IUsersHasTaskRepository _usersHasTaskRepository;

        public TaskController(ITaskRepository taskRepository, IUsersHasTaskRepository usersHasTaskRepository)
        {
            _taskrepository = taskRepository;
            _usersHasTaskRepository = usersHasTaskRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var res = _taskrepository.GetAll();
            return Ok(res);
        }

        [HttpGet("/api/[controller]/{projectId}")]
        public async Task<IActionResult> GetTaskByProjectId(string projectId, int filter)
        {
            var res = _taskrepository.GetByPredicate(x => x.ProjectId == long.Parse(projectId) && (filter == -1 || x.State == filter));
            foreach(Dbo.Task task in res)
            {
                var colls = await _usersHasTaskRepository.GetTaskCollaborators(task.Id);
                task.collaborators = colls.ToList();
            }
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

        [HttpGet("/api/[controller]/{taskId}/Users")]
        public async Task<IActionResult> GetTaskCollaborators(string taskId)
        {
            var res = await _usersHasTaskRepository.GetTaskCollaborators(long.Parse(taskId));
            return Ok(res);
        }

        [HttpPost("/api/[controller]/Users")]
        public async Task<IActionResult> AddTaskCollaborators(Dbo.UsersHasTask usersHasTask)
        {
            var res = await _usersHasTaskRepository.Insert(usersHasTask);
            return Ok(res);
        }

        [HttpDelete("/api/[controller]/{taskId}/User/{userId}")]
        public async Task<IActionResult> DeleteTaskCollaborator(string taskId, string userId)
        {
            var res = await _usersHasTaskRepository.removeUserFromTask(long.Parse(taskId), long.Parse(userId));
            return Ok(res);
        }
    }
}
