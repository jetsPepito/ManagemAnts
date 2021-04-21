using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    public static class ControllerExtension
    {
        public static string Url(this Controller controller) =>
            "https://localhost:44352/api/";

        public static long UserId(this Controller controller) =>
            long.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public static string UserName(this Controller controller) =>
            controller.User.FindFirstValue(ClaimTypes.Name);

        public static string UserPseudo(this Controller controller) =>
            controller.User.FindFirstValue(ClaimTypes.GivenName);

        public static Models.User GetLoggedUser(this Controller controller)
        {
            var user = new Models.User()
            {
                id = UserId(controller),
                pseudo = UserPseudo(controller),
            };

            return user;
        }

        public static HttpClient SetUpClient(this Controller controller, string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Url(controller) + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }


    }
}
