using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagemAntsServer;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest.Utils
{
    class TaskUtils
    {
        public static bool IsEqualTasks(Task a, ManagemAntsServer.Dbo.Task b)
        {
            return a.Id == b.Id &&
                a.Name == b.Name &&
                a.Description == b.Description &&
                a.CreatedAt == b.CreatedAt &&
                a.Duration == b.Duration &&
                a.State == b.State &&
                a.ProjectId == b.ProjectId &&
                a.TimeSpent == b.TimeSpent;
        }
        public static bool IsEqualTasks(ManagemAntsServer.Dbo.Task a, ManagemAntsServer.Dbo.Task b)
        {
            return a.Id == b.Id &&
                a.Name == b.Name &&
                a.Description == b.Description &&
                a.CreatedAt == b.CreatedAt &&
                a.Duration == b.Duration &&
                a.State == b.State &&
                a.ProjectId == b.ProjectId &&
                a.TimeSpent == b.TimeSpent;
        }
        public static bool IsEqualUserHasTasks(UsersHasTask a, ManagemAntsServer.Dbo.UsersHasTask b)
        {
            return a.Id == b.Id &&
                a.TaskId == b.TaskId &&
                a.UserId == b.UserId;
        }
        public static bool IsEqualUserHasTasks(ManagemAntsServer.Dbo.UsersHasTask a, ManagemAntsServer.Dbo.UsersHasTask b)
        {
            return a.Id == b.Id &&
                a.TaskId == b.TaskId &&
                a.UserId == b.UserId;
        }
    }
}
