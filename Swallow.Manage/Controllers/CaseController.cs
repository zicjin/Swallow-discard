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
    public class CaseController : Controller {
        private int page_size = 999;

        private readonly ICaseDbForManage CaseDb;
        public CaseController(ICaseDbForManage db) {
            this.CaseDb = db;
        }

        public IActionResult Item(string id) {
            var user = CaseDb.Get(id);
            return View(user);
        }

        public IActionResult Index(
            CaseStatus status = CaseStatus.All,
            string userId = null,
            string articleId = null,
            SortPattern pattern = SortPattern.Newest,
            string query = null,
            int page = 1
        ) {
            var users = CaseDb.Index(status, userId, articleId, query, pattern, page, page_size);
            ViewBag.Query = query;
            ViewBag.StatusSel = status.ToSelectListItems();
            ViewBag.UserId = userId;
            ViewBag.ArticleId = articleId;
            ViewBag.PatternSel = pattern.ToSelectListItems();
            return View(users.ToList()); // PagedList don't support asp.net5 about Razor so far
        }

        private void BuildCreateView() {
            ViewBag.StatusSel = ArticleStatus.Unapproved.ToSelectListItems();
            ViewBag.TypeSel = ArticleType.Unknow.ToSelectListItems();
            ViewBag.VectorSel = ArticleVector.Text.ToSelectListItems();
        }

        public ActionResult Create() {
            BuildCreateView();
            return View(new Case());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Case model) {
            BuildCreateView();
            ModelState.Remove("CreatDate");
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            string failure;
            var user = CaseDb.Create(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "新建失败");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        private void BuildEditView(Case article) {
            ViewBag.StatusSel = ((CaseStatus)article.Status).ToSelectListItems();
        }

        public ActionResult Edit(string id) {
            var _case = CaseDb.Get(id);
            BuildEditView(_case);
            return View(_case);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Case model) {
            BuildEditView(model);
            ModelState.Remove("CreatDate");
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            string failure;
            var user = CaseDb.Update(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "更新失败");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id, string failure) {
            var user = CaseDb.Get(id);
            ViewBag.Failure = failure;
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            string failure;
            CaseDb.Delete(id, out failure);
            if (!string.IsNullOrEmpty(failure))
                return RedirectToAction("Delete", new { id = id, failure = failure });
            return RedirectToAction("Index");
        }
    }
}