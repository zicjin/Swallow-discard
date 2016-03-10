using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Swallow.Entity;
using Microsoft.AspNet.Authorization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PagedList;
using Swallow.Core;

namespace Swallow.Manage.Controllers {
    [Authorize]
    public class UserController : Controller {
        private int page_size = 999;

        private readonly IUserDb UserDb;
        public UserController(IUserDb db) {
            this.UserDb = db;
        }

        public IActionResult Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, int page = 1, string query = null) {
            var users = UserDb.Index(status, pattern, query, page, page_size);
            ViewBag.Query = query;
            return View(users.ToList()); // PagedList don't support asp.net5 about Razor so far
        }
    }
}
