using ManagemAntsClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly string test;
        private string url = "https://localhost:44352/api/Project";

        static HttpClient client = new HttpClient();
        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*            client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage responce = client.GetAsync("").Result;

                        IEnumerable<Project> projects = null;
                        if (responce.IsSuccessStatusCode)
                            projects = await JsonSerializer.DeserializeAsync<IEnumerable<Project>>(await responce.Content.ReadAsStreamAsync());
                        return View(new Projects(projects));*/

            var tasks = new List<Models.Task>();
            tasks.Add(new Models.Task() { Id = 0, Name = "Faire des pates a la bolo", Description = "Une description", State = 0 });
            tasks.Add(new Models.Task() { Id = 1, Name = "Faire le .NET", Description = "Une description", State = 1 });
            tasks.Add(new Models.Task() { Id = 2, Name = "Faire vue js", Description = "Une description", State = 2 });
            tasks.Add(new Models.Task() { Id = 3, Name = "Rendre les fichiers de .NET", Description = "Une description", State = 3 });


            return View(
                new ProjectPage() { 
                    Project = new Project() { id = 1, name = "Ouistiti", description = "Un ptit singe tout mimi" },
                    LoggedUser = new User() { Pseudo = "Kaijo", Firstname = "Jeremie", Lastname = "Zeitoun" },
                    Tasks = tasks
                });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
