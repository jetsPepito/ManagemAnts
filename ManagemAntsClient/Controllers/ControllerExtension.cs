using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManagemAntsClient.Controllers
{
    public static class ControllerExtension
    {
        public static string UserId(this Controller controller) =>
            controller.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string UserName(this Controller controller) =>
            controller.User.FindFirstValue(ClaimTypes.Name);
    }
}
