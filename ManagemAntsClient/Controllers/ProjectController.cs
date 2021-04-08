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
            return View(new ProjectPage() { project = new Project() { id = 1, name = "Ouistiti", description = "Un ptit singe tout mimi" }, loggedUser = "Kaijo" }) ;
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
