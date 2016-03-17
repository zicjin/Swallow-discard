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
    public class ArticleController : Controller {
        private int page_size = 999;

        private readonly IArticleDbForManage ArticleDb;
        public ArticleController(IArticleDbForManage db) {
            this.ArticleDb = db;
        }

        public IActionResult Item(string id) {
            var user = ArticleDb.Get(id);
            return View(user);
        }

        public IActionResult Index(
            ArticleStatus status = ArticleStatus.All,
            ArticleType type = ArticleType.All,
            ArticleVector vector = ArticleVector.All,
            string query = null,
            SortPattern pattern = SortPattern.Newest,
            int page = 1
        ) {
            var users = ArticleDb.Index(status, type, vector, query, pattern, page, page_size);
            ViewBag.Query = query;
            ViewBag.StatusSel = status.ToSelectListItems();
            ViewBag.TypeSel = type.ToSelectListItems();
            ViewBag.VectorSel = vector.ToSelectListItems();
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
            return View(new Article());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article model) {
            BuildCreateView();
            ModelState.Remove("CreatDate");
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            string failure;
            model.UserId = User.Identity.Name;
            var user = ArticleDb.Create(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "新建失败");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        private void BuildEditView(Article article) {
            ViewBag.StatusSel = ((ArticleStatus)article.Status).ToSelectListItems();
            ViewBag.TypeSel = ((ArticleType)article.Type).ToSelectListItems();
            ViewBag.VectorSel = ((ArticleVector)article.Vector).ToSelectListItems();
        }

        public ActionResult Edit(string id) {
            var article = ArticleDb.Get(id);
            BuildEditView(article);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Article model) {
            BuildEditView(model);
            ModelState.Remove("CreatDate");
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            string failure;
            var user = ArticleDb.Update(model, out failure);
            if (!string.IsNullOrEmpty(failure) || user == null) {
                ModelState.AddModelError("", failure ?? "更新失败");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id, string failure) {
            var user = ArticleDb.Get(id);
            ViewBag.Failure = failure;
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            string failure;
            ArticleDb.Delete(id, out failure);
            if (!string.IsNullOrEmpty(failure))
                return RedirectToAction("Delete", new { id = id, failure = failure});
            return RedirectToAction("Index");
        }
    }
}