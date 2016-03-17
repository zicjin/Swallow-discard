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

        public IActionResult Item(string id) {
            var user = UserDb.Get(id);
            return View(user);
        }

        public IActionResult Index(UserStatus status = UserStatus.All, string query = null, SortPattern pattern = SortPattern.Newest, int page = 1) {
            var users = UserDb.Index(status, query, pattern, page, page_size);
            ViewBag.StatusSel = status.ToSelectListItems();
            ViewBag.Query = query;
            return View(users.ToList()); // PagedList don't support asp.net5 about Razor so far
        }

        public ActionResult IndexLineOn() {
            return this.View();
        }

        public ActionResult Create() {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User model, string RolesParams) {
            ModelState.Remove("CreatDate");
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }

            string failure;
            var user = UserDb.Create(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "新建失败");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        private void BuildEditView(User user) {
            ViewBag.StatusSel = ((UserStatus)user.Status).ToSelectListItemsFilterNull();
        }

        public ActionResult Edit(string id) {
            var user = UserDb.Get(id);
            BuildEditView(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model, string RolesParams) { // [Bind(Exclude = "Password,CreatDate")]
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