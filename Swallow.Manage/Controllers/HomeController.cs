using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Swallow.Manage.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authorization;
using AspNet.Identity3.MongoDB;

namespace Swallow.Manage.Controllers {
    public class HomeController : Controller {

        public IActionResult Index() {
            return View();
        }

        public IActionResult Error() {
            return View();
        }
    }
}
