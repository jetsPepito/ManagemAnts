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
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LoginController( IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet("/api/[controller]")]
        public IActionResult Login(string pseudo, string passwordHash)
        {
            var user = _userRepository.GetByPredicate(x => x.Pseudo == pseudo && x.Password == passwordHash).FirstOrDefault();

            return Ok(user);
        }

        [HttpPost("")]
        public async Task<IActionResult> SignUp(Dbo.User user)
        {
            var alreadyExist = _userRepository.GetByPredicate(x => x.Pseudo == user.Pseudo).FirstOrDefault();
            if (alreadyExist != null)
                return Ok(null);

            var result = _userRepository.Insert(user);

            return Ok(result);
        }
    }
}
