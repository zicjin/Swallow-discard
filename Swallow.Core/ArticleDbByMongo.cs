using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PagedList;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Core {
    public class ArticleDbByMongo: IArticleDbForManage {
        private readonly IMongoCollection<Article> Db;

        public ArticleDbByMongo(MongoDbContext mongo) {
            this.Db = mongo.Articles;
        }

        public IPagedList<Article> Index(
            ArticleStatus status = ArticleStatus.All,
            ArticleType type = ArticleType.All,
            ArticleVector vector = ArticleVector.All,
            string query = null,
            SortPattern pattern = SortPattern.Newest,
            int page = 1,
            int page_size = 30
        ) {
            var articles = Db.AsQueryable();

            if (status != ArticleStatus.All)
                articles = articles.Where(d => d.Status == status);
            if (type != ArticleType.All)
                articles = articles.Where(d => d.Type == type);
            if (vector != ArticleVector.All)
                articles = articles.Where(d => d.Vector == vector);

            if (!string.IsNullOrEmpty(query))
                articles = articles.Where(d => d.UserId == query || d.Title == query || d. Introduction == query);

            articles = pattern.ArticleOrderBy()(articles);

            return articles.ToPagedList(page, page_size);
        }

        public Article Get(string id) {
            return Db.AsQueryable().Where(d => d.Id == id).SingleOrDefault();
        }

        public Article Create(Article model, out string failure) {
            if (!EntityValidator.TryValidate(model, null, out failure))
                return null;
            model.Create();
            Db.InsertOne(model);
            return model;
        }

        #region ForManage
        public Article Update(Article model, out string failure) {
            if (!EntityValidator.TryValidate(model, null, out failure))
                return null;

            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == model.Id,
                model,
                new UpdateOptions { IsUpsert = true });

            if (result.IsAcknowledged)
                return model;
            return null;
        }

        public void Delete(string id, out string failure) {
            failure = string.Empty;
            DeleteResult result = Db.DeleteOne(
                d => d.Id == id
            );
            if (result.IsAcknowledged) {
                failure = "删除失败";
                return;
            }
            return;
        }
        #endregion
    }
}
