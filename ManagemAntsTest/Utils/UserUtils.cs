using ManagemAntsServer.DataAccess.EfModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagemAntsTest.Utils
{
    public static class UserUtils
    {
        public static bool IsEqualUsers(User a, ManagemAntsServer.Dbo.User b)
        {
            return a.Id == b.Id && 
                a.Pseudo == b.Pseudo &&
                a.Firstname == b.Firstname &&
                a.Lastname == b.Lastname &&
                a.Password == b.Password;
        }
        public static bool IsEqualUsers(ManagemAntsServer.Dbo.User a, ManagemAntsServer.Dbo.User b)
        {
            return a.Id == b.Id && 
                a.Pseudo == b.Pseudo &&
                a.Firstname == b.Firstname &&
                a.Lastname == b.Lastname &&
                a.Password == b.Password;
        }
    }
}
