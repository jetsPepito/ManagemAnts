using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public class UsersHasTask : IObjectWithId
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TaskId { get; set; }

        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
    }
}
