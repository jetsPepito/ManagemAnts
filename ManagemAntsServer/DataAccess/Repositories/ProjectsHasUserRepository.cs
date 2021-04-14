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
    }
}
