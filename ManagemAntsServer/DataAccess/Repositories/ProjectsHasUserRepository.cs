using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class ProjectsHasUserRepository: Repository<EfModels.ProjectsHasUser, Dbo.ProjectsHasUser>, Interfaces.IProjectsHasUserRepository
    {
        public ProjectsHasUserRepository(EfModels.ManagemAntsDbContext context, ILogger<TaskRepository> logger, IMapper mapper) : base(context, logger, mapper)
        { }
    }
}
