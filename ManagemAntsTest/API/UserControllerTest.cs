using NUnit.Framework;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsServer.Controllers;
using System;
using AutoMapper;
using ManagemAntsServer.DataAccess.EfModels;
using System.Collections.Generic;
using System.Linq;
using ManagemAntsTest.Utils;

namespace ManagemAntsTest.API
{
    public class UserControllerTest : BaseSetup
    {
        private UserController _userController;

        [SetUp]
        public void Setup()
        {
            SetupDb();
            var myProfile = new AutomapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            UserRepository userRepository = new UserRepository(contextMock.Object, null, mapper);
            _userController = new UserController(userRepository);
        }

        [Test]
        public void GetUsers()
        {
            var result = _userController.Get() as Microsoft.AspNetCore.Mvc.OkObjectResult;
            
            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(_refFixture.Users.Count, users.Length);
        }

        [Test]
        public void GetUserById()
        {
            var userToGet = _refFixture.Users[0];
            var result = _userController.GetById(userToGet.Id.ToString()) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            
            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(1, users.Length);
            Assert.IsTrue(UserUtils.IsEqualUsers(userToGet, users[0]));
        }

        [Test]
        public void GetUserByIdUnknown()
        {
            var result = _userController.GetById("-1") as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(0, users.Length);
        }

        [Test]
        public void GetUserByFilter()
        {
            var filter = _refFixture.Users[0].Pseudo.Substring(2).ToLower();
            var usersToGet = _refFixture.Users.Where(
                    x => x.Firstname.ToLower().Contains(filter)
                    || x.Lastname.ToLower().Contains(filter)
                    || x.Pseudo.ToLower().Contains(filter)).ToList();

            var result = _userController.GetByFilter(filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(usersToGet.Count(), users.Length);
            for (int i = 0; i < users.Length; i++)
            {
                Assert.IsTrue(UserUtils.IsEqualUsers(usersToGet[i], users[i]));
            }
        }

        [Test]
        public void GetUserByEmptyFilter()
        {
            var filter = "";
            var usersToGet = _refFixture.Users.ToList();

            var result = _userController.GetByFilter(filter) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(usersToGet.Count(), users.Length);
            for (int i = 0; i < users.Length; i++)
            {
                Assert.IsTrue(UserUtils.IsEqualUsers(usersToGet[i], users[i]));
            }
        }

        [Test]
        public void GetUserByPseudo()
        {
            var userToGet = _refFixture.Users[0];
            var result = _userController.GetByPseudo(userToGet.Pseudo) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(1, users.Length);
            Assert.IsTrue(UserUtils.IsEqualUsers(userToGet, users[0]));
        }

        [Test]
        public void GetUserByPseudoFail()
        {
            var result = _userController.GetByPseudo("THISUSERDOESNOTEXISTS") as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.AreEqual(200, result.StatusCode);

            var users = result.Value as ManagemAntsServer.Dbo.User[];
            Assert.AreEqual(0, users.Length);
        }
    }
}