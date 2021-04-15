using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class UsersHasTaskRepository : Repository<EfModels.UsersHasTask, Dbo.UsersHasTask>, Interfaces.IUsersHasTaskRepository
    {
        public UsersHasTaskRepository(EfModels.ManagemAntsDbContext context, ILogger<UsersHasTaskRepository> logger, IMapper mapper) : base(context, logger, mapper)
        { }
        public virtual async Task<IEnumerable<Dbo.User>> GetTaskCollaborators(long taskId)
        {
            var agr = _set.AsQueryable()
                            .Include(x => x.User)
                            .AsEnumerable()
                            .Where(x => x.TaskId == taskId);

            var users = new List<Dbo.User>();

            foreach (var el in agr)
            {
                users.Add(_mapper.Map<Dbo.User>(el.User));
            }

            return users;
        }

        public virtual async Task<bool> removeUserFromTask(long taskId, long userId)
        {
            IEnumerable<EfModels.UsersHasTask> dbEntity = _set.Where(x => x.TaskId == taskId && x.UserId == userId);


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
        public virtual async Task<bool> removeUserFromTasks(List<long> taskIds, long userId)
        {
            IEnumerable<EfModels.UsersHasTask> dbEntity = _set.Where(x => taskIds.Contains(x.TaskId) && x.UserId == userId);


            if (dbEntity.Count() == 0)
            {
                return false;
            }
            foreach (var id in dbEntity)
                _set.Remove(id);
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
