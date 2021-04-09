using ManagemAntsServer.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsServer.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var x =  _userRepository.GetAll();
            return Ok(x);
        }

        [HttpGet("/api/[controller]/{id}")]
        public IActionResult GetById(string id)
        {
            var result = _userRepository.GetByPredicate(x => x.Id == long.Parse(id));
            return Ok(result);
        }

        [HttpGet("/api/[controller]/research/{filter}")]
        public IActionResult GetByFilter(string filter)
        {
            var filterLower = filter.ToLower();
            var result = _userRepository.GetByPredicate(
                    x => x.Firstname.ToLower().Contains(filterLower)
                    || x.Lastname.ToLower().Contains(filterLower)
                    || x.Pseudo.ToLower().Contains(filterLower));
            return Ok(result);
        }
    }
}
