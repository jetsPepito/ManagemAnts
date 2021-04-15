using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class ProjectsHasUserRepository: Repository<EfModels.ProjectsHasUser, Dbo.ProjectsHasUser>, Interfaces.IProjectsHasUserRepository
    {
        public ProjectsHasUserRepository(EfModels.ManagemAntsDbContext context, ILogger<TaskRepository> logger, IMapper mapper) : base(context, logger, mapper)
        { 
        }

        public virtual async Task<IEnumerable<Dbo.Project>> GetProjectByUserId(long id)
        {
  
            var agr = _set.AsQueryable()
                            .Include(x => x.Project)
                            .AsEnumerable()
                            .Where(x => x.UserId == id);

            var projects = new List<Dbo.Project>();

            foreach(var el in agr)
            {
                projects.Add(_mapper.Map<Dbo.Project>(el.Project));
            }

            return projects;

        }

        public virtual async Task<IEnumerable<Dbo.User>> GetProjectCollaborators(long projectId)
        {
            var agr = _set.AsQueryable()
                            .Include(x => x.User)
                            .AsEnumerable()
                            .Where(x => x.ProjectId == projectId);

            var users = new List<Dbo.User>();

            foreach (var el in agr)
            {
                users.Add(_mapper.Map<Dbo.User>(el.User));
            }

            return users;
        }

        public virtual async Task<bool> removeUserFromProject(long projectId, long userId)
        {
            IEnumerable<EfModels.ProjectsHasUser> dbEntity = _set.Where(x => x.ProjectId == projectId && x.UserId == userId);


            if (dbEntity.Count() == 0)
            {
                return false;
            }
            _set.Remove(dbEntity.FirstOrDefault());
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return false;
            }
        }
    }
}
