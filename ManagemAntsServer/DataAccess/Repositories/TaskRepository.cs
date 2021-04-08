using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class TaskRepository : Repository<EfModels.Task, Dbo.Task>, Interfaces.ITaskRepository
    {
        public TaskRepository(EfModels.ManagemAntsDbContext context, ILogger<TaskRepository> logger, IMapper mapper) : base(context, logger, mapper)
        { }
    }
}
