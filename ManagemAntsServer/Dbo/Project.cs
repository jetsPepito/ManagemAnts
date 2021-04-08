﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Dbo
{
    public class Project: IObjectWithId
    {
        public Project()
        {
            Tasks = new HashSet<Task>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Owner { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
