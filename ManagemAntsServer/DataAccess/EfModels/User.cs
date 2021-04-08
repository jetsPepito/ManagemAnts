﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ManagemAntsServer.DataAccess.EfModels
{
    public partial class User
    {
        public User()
        {
            Projects = new HashSet<Project>();
        }

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Pseudo { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}