using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Interfaces
{
    public interface ITaskRepository: DataAccess.Repositories.IRepository<EfModels.Task, Dbo.Task>
    {

    }
}
