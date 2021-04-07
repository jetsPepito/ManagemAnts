using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface IProjectRepository: Repositories.IRepository<EfModels.Project, Dbo.Project>
    {

    }
}
