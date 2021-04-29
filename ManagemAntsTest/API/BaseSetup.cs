using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagemAntsServer.DataAccess.EfModels;

namespace ManagemAntsTest.API
{
    public class BaseSetup
    {
        protected Mock<ManagemAntsDbContext> contextMock;
        protected Fixtures _refFixture;

        protected void SetupDb()
        {
            contextMock = new Mock<ManagemAntsDbContext>();
            var database = new DbSetup();
            contextMock.Setup(s => s.Set<User>()).Returns(database.UsersSet);
            contextMock.Setup(s => s.Users).Returns(database.UsersSet);
            _refFixture = new Fixtures();
        }
    }
}
