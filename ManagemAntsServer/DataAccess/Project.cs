using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess
{
    public partial class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Owner { get; set; }
    }
}
