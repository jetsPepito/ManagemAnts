using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public interface IObjectWithId
    {
        long Id { get; set; }
    }
}
