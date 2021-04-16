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

        public virtual async Task<IEnumerable<Dbo.UserWithRole>> GetProjectCollaborators(long projectId)
        {
            var agr = _set.AsQueryable()
                            .Include(x => x.User)
                            .AsEnumerable()
                            .Where(x => x.ProjectId == projectId);

            var users = new List<Dbo.UserWithRole>();

            foreach (var el in agr)
            {
                var userWithRole = new Dbo.UserWithRole()
                {
                    Id = el.User.Id,
                    Firstname = el.User.Firstname,
                    Lastname = el.User.Lastname,
                    Pseudo = el.User.Pseudo,
                    Role = el.Role
                };

                users.Add(userWithRole);
            }

            return users;
        }

        public virtual async Task<IEnumerable<Dbo.User>> GetProjectCollaboratorsByRole(long projectId, int roleValue)
        {
            var agr = _set.AsQueryable()
                            .Include(x => x.User)
                            .AsEnumerable()
                            .Where(x => x.ProjectId == projectId && x.Role == roleValue);

            var users = new List<Dbo.User>();

            foreach (var el in agr)
            {
                users.Add(_mapper.Map<Dbo.User>(el.User));
            }

            return users;
        }

        public virtual IEnumerable<Dbo.User> GetProjectCollaboratorsByFilter(long projectId, string filter)
        {
            var filterLower = filter.ToLower();
            var projectsHasUsers = _set.AsQueryable()
                            .Include(x => x.User)
                            .AsEnumerable()
                            .Where(x => x.ProjectId == projectId
                                    && (x.User.Firstname.ToLower().Contains(filterLower)
                                    || x.User.Lastname.ToLower().Contains(filterLower)
                                    || x.User.Pseudo.ToLower().Contains(filterLower)));

            var users = new List<Dbo.User>();

            foreach (var el in projectsHasUsers)
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
