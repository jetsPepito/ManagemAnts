using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface IProjectsHasUserRepository: Repositories.IRepository<EfModels.ProjectsHasUser, Dbo.ProjectsHasUser>
    {
        Task<IEnumerable<Dbo.Project>> GetProjectByUserId(long id, string searchFilter);
        Task<IEnumerable<Dbo.UserWithRole>> GetProjectCollaborators(long projectId);
        Task<IEnumerable<Dbo.User>> GetProjectCollaboratorsByRole(long projectId, int roleValue);
        Task<bool> removeUserFromProject(long projectId, long userId);
        IEnumerable<Dbo.User> GetProjectCollaboratorsByFilter(long projectId, string filter);
    }
}
