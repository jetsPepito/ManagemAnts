using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.DataAccess;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public class UserRepository: Repository<EfModels.User, Dbo.User>, Interfaces.IUserRepository
    {
        public UserRepository(EfModels.ManagemAntsDbContext context, ILogger<TaskRepository> logger, IMapper mapper) : base(context, logger, mapper)
        { }
    }
}
