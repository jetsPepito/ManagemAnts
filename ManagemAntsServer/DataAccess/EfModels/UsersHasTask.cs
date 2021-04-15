using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess.EfModels
{
    public partial class UsersHasTask
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TaskId { get; set; }

        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
    }
}
