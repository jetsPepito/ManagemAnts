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
            var users = result.Value as User[];
            Assert.AreEqual(2, users.Length);
        }
    }
}