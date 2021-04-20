using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    public class ConnexionController : Controller
    {
        // GET: ConnexionController
        public ActionResult Index()
        {
            return View();
        }
    }
}
