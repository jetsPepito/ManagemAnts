using AutoMapper;
using ManagemAntsServer.Controllers;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsTest.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagemAntsTest.API
{
    public class LoginControllerTest : BaseSetup
    {
        private LoginController _loginController;

        [SetUp]
        public void Setup()
        {
            SetupDb();
            var myProfile = new AutomapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            UserRepository userRepository = new UserRepository(contextMock.Object, null, mapper);
            _loginController = new LoginController(userRepository);
        }

        [Test]
        public void Login()
        {
            var userToGet = _refFixture.Users[0];
            var result = _loginController.Login(userToGet.Pseudo) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(1, users.Length);
            Assert.True(UserUtils.IsEqualUsers(userToGet, users[0]));
        }

        [Test]
        public void LoginFail()
        {
            var result = _loginController.Login("THISUSERDOESNOTEXISTS") as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(0, users.Length);
        }

        [Test]
        public async Task SignUp()
        {
            var newUser = new ManagemAntsServer.Dbo.User()
            {
                Pseudo = "ThisIsANewPseudo",
                Firstname = "ThisIsANewFirstaname",
                Lastname = "ThisIsANewLastname",
                Password = "testtest"
            };
            var result = await _loginController.SignUp(newUser) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var addedUserResult = _loginController.Login(newUser.Pseudo) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var users = addedUserResult.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(1, users.Length);
            Assert.IsTrue(UserUtils.IsEqualUsers(newUser, users[0]));
        }

        [Test]
        public async Task SignUpAlreadyExist()
        {
            var prevCount = _refFixture.Users.Count;
            var newUser = new ManagemAntsServer.Dbo.User()
            {
                Pseudo = _refFixture.Users[0].Pseudo,
                Firstname = "ThisIsANewFirstaname",
                Lastname = "ThisIsANewLastname",
                Password = "testtest"
            };
            var result = await _loginController.SignUp(newUser) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var addedUserResult = _loginController.Login(newUser.Pseudo) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var users = addedUserResult.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(1, users.Length);
            Assert.IsFalse(UserUtils.IsEqualUsers(newUser, users[0]));
        }
    }
}
