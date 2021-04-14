using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface IProjectsHasUserRepository: Repositories.IRepository<EfModels.ProjectsHasUser, Dbo.ProjectsHasUser>
    {
        Task<IEnumerable<Dbo.Project>> GetProjectByUserId(long id);
        Task<IEnumerable<Dbo.User>> GetProjectCollaborators(long projectId);
    }
}
