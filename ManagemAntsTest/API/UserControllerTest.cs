using NUnit.Framework;
using ManagemAntsServer.DataAccess;
using ManagemAntsServer.DataAccess.Repositories;
using ManagemAntsServer.Controllers;
using System;
using AutoMapper;
using ManagemAntsServer.DataAccess.EfModels;
using System.Collections.Generic;

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
            Assert.IsTrue(IsEqualUsers(userToGet, users[0]));
        }
        
        public bool IsEqualUsers(User a, ManagemAntsServer.Dbo.User b)
        {
            return a.Pseudo == b.Pseudo &&
                a.Firstname == b.Firstname &&
                a.Lastname == b.Lastname &&
                a.Password == b.Password;
        }
    }
}