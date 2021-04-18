using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface IUsersHasTaskRepository : Repositories.IRepository<EfModels.UsersHasTask, Dbo.UsersHasTask>
    {
        Task<IEnumerable<Dbo.User>> GetTaskCollaborators(long taskId);
        Task<bool> removeUserFromTask(long taskId, long userId);
        Task<bool> removeUserFromTasks(List<long> taskIds, long userId);
    }
}
