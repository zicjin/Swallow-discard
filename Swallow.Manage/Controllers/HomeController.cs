using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Swallow.Core;

namespace Swallow.Manage.Controllers {
    public class HomeController : Controller {
        MongoDbContext context;
        public HomeController(MongoDbContext _context) {
            context = _context;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Error() {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult SeedDb() {
            DbSeed.MongoDB(context);
            return new JsonResult(new { msg = "success" });
        }
    }
}
