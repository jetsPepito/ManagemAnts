using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess
{
    public class AutomapperProfiles: Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Dbo.Project, EfModels.Project>();
            CreateMap<EfModels.Project, Dbo.Project>();

            CreateMap<Dbo.Task, EfModels.Task>();
            CreateMap<EfModels.Task, Dbo.Task>();

        }
    }
}
