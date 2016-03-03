using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Swallow.Entity;
using Microsoft.AspNet.Authorization;
using MongoDB.Driver;
using Swallow.Manage.Models;
using MongoDB.Driver.Linq;

namespace Swallow.Manage.Controllers {
    [Authorize]
    public class UserController : Controller {
        private readonly IMongoQueryable<User> UserStore;
        public UserController(AppDbContext db) {
            this.UserStore = db.Users.AsQueryable();
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index() {
            var users = await UserStore.ToListAsync();
            return View(users);
        }
    }
}
