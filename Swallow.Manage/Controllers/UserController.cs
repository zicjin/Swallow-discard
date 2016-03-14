using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Swallow.Entity;
using Microsoft.AspNet.Authorization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Swallow.Core;

namespace Swallow.Manage.Controllers {
    [Authorize]
    public class UserController : Controller {
        private int page_size = 999;

        private readonly IUserDbForManage UserDb;
        public UserController(IUserDbForManage db) {
            this.UserDb = db;
        }

        public IActionResult Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, int page = 1, string query = null) {
            var users = UserDb.Index(status, pattern, query, page, page_size);
            ViewBag.Query = query;
            return View(users.ToList()); // PagedList don't support asp.net5 about Razor so far
        }

        public ActionResult IndexLineOn() {
            return this.View();
        }

        public ActionResult Search(string query) {
            var list = new List<User>();
            var users = UserDb.Index(UserStatus.All, SortPattern.Newest, query);
            return View("Index", users.ToList());
        }

        private void BuildEditView(User user) {
            ViewBag.StatusSel = ((UserStatus)user.Status).ToSelectListItems();
        }

        public ActionResult Edit(string id) {
            var user = UserDb.Get(id);
            BuildEditView(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model) { // [Bind(Exclude = "Password,CreatDate")]
            ModelState.Remove("Password");
            ModelState.Remove("CreatDate"); // http://stackoverflow.com/a/25314838/346701
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                BuildEditView(model);
                return View(model);
            }
            string failure;
            var user = UserDb.Update(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "更新失败");
                BuildEditView(model);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id, string failure) {
            var user = UserDb.Get(id);
            ViewBag.Failure = failure;
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            string failure;
            UserDb.Delete(id, out failure);
            if (!string.IsNullOrEmpty(failure))
                return RedirectToAction("Delete", new { id = id, failure = failure});
            return RedirectToAction("Index");
        }
    }
}