using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface IProjectsHasUserRepository: Repositories.IRepository<EfModels.ProjectsHasUser, Dbo.ProjectsHasUser>
    {
    }
}
