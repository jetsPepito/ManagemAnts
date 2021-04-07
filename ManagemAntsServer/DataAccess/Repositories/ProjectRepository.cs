using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class ProjectRepository: Repository<EfModels.Project, Dbo.Project>, Interfaces.IProjectRepository
    {
        public ProjectRepository(EfModels.ManagemAntsDbContext context, ILogger<ProjectRepository> logger, IMapper mapper): base(context, logger, mapper)
        { }
    }
}
