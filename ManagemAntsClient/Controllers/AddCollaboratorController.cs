using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    public class AddCollaboratorController : Controller
    {
        private string url = "https://localhost:44352/api/";

        private HttpClient SetUpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url + endpoint);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        // GET: AddCollaboratorController
        public async Task<ActionResult> Index(string projectId, string projectName, string userId)
        {
            var client = SetUpClient("User/" + userId);
            HttpResponseMessage response = client.GetAsync("").Result;

            var user = new Models.User();

            Models.AddCollaboratorPage addCollaboratorPage = new Models.AddCollaboratorPage()
            {
                ProjectId = projectId,
                ProjectName = projectName,
                LoggedUser = user
            };

            return View(addCollaboratorPage);
        }

        // GET: AddCollaboratorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AddCollaboratorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddCollaboratorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AddCollaboratorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AddCollaboratorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AddCollaboratorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AddCollaboratorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
