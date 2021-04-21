using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ManagemAntsClient.Controllers
{
    // return code 
    // 1 -> registered
    // 2 -> pseudo already exist
    // 3 -> passwords not equal
    // 4 -> wrong pseudo or password


    public class ConnexionController : Controller
    {
        private readonly ILogger<ConnexionController> _logger;
        private string url = "https://localhost:44352/api/";

        public ConnexionController(ILogger<ConnexionController> logger)
        {
            _logger = logger;
        }


        private HttpClient SetUpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // GET: ConnexionController
        public ActionResult Index()
        {
            var uri = new Uri(HttpContext.Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(uri.Query);
            
            
            var returnCode = query.Get("returnCode");

            if (returnCode == null)
                returnCode = "0";

            Models.Login login = new Models.Login
            {
                status = (Models.Login.statusEnum)int.Parse(returnCode),
            };

            return View(login);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> loginAsync(string pseudo, string password)
        {

            HttpClient client = SetUpClient("Login?pseudo=" + pseudo);

            HttpResponseMessage response = client.GetAsync("").Result;

            Models.User user = null;

            var userList = new List<Models.User>();

            if (response.IsSuccessStatusCode)
            {
                userList = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
                if (userList.Count == 0)
                {
                    //user not found
                    return RedirectToAction("Index", "Connexion", new { returnCode = 4});
                }

                user = userList[0];
            }

            // check password
            if (!BCrypt.Net.BCrypt.Verify(password, user.password))
            {
                //wrong password
                return RedirectToAction("Index", "Connexion", new { returnCode = 4 });
            }
                

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.firstname} {user.lastname}"),
                    new Claim(ClaimTypes.GivenName, $"{user.pseudo}")
                },
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in and redirect to home page
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });
            return RedirectToAction("Index", "Dashboard");

        }

        public async Task<IActionResult> signUpAsync(string lastname, string firstname, string pseudo, string password, string verification_password)
        {
            if (password != verification_password) // password not equal
                return RedirectToAction("Index", "Connexion", new { returnCode = 3});

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            HttpClient client = SetUpClient("Login");

            var user = new Models.User()
            {
                firstname = firstname,
                lastname = lastname,
                pseudo = pseudo,
                password = passwordHash
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = JsonContent.Create(user)
            };

            var response = await client.SendAsync(postRequest);

            Models.User responseUser = null;

            var userList = new List<Models.User>();

            if (response.IsSuccessStatusCode)
            {

                userList = await JsonSerializer.DeserializeAsync<List<Models.User>>(await response.Content.ReadAsStreamAsync());
                if (userList.Count == 0)
                {
                    //user already exist
                    return RedirectToAction("Index", "Connexion", new { returnCode = 2});
                }

                responseUser = userList[0];
            }

            // yes registered
            return RedirectToAction("Index", "Connexion", new { returnCode = 1});
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Connexion");
        }

    }
}
