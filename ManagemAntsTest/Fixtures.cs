using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest
{
    public class Fixtures
    {
        public List<User> Users;

        public Fixtures()
        {
            setUsers();
        }

        private void setUsers()
        {
            Users = new List<User>
            {
                new User()
                {
                    Id = 1,
                    Firstname = "Toto",
                    Lastname = "Titi",
                    Pseudo = "TT",
                    Password = "1234"
                },
                new User()
                {
                    Id = 2,
                    Firstname = "Tata",
                    Lastname = "Tutu",
                    Pseudo = "TT2",
                    Password = "azerty"
                },
            };
        }
    }
}
