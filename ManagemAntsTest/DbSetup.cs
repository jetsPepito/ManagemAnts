using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest
{
    public class DbSetup
    {
        private readonly Fixtures _fixtures;
        public DbSet<User> UsersSet => GetMockDbSet(_fixtures.Users);
        public DbSet<Project> ProjectsSet => GetMockDbSet(_fixtures.Projects);
        public DbSet<ProjectsHasUser> ProjectHasUsersSet => GetMockDbSet(_fixtures.ProjectsHasUsers);
        public DbSet<Task> TasksSet => GetMockDbSet(_fixtures.Tasks);
        public DbSet<UsersHasTask> UsersHasTasksSet => GetMockDbSet(_fixtures.UsersHasTasks);

        public DbSetup()
        {
            _fixtures = new Fixtures();
        }
        private DbSet<T> GetMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(m => m.AsQueryable()).Returns(queryable);

            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

            return dbSet.Object;
        }
    }
}
