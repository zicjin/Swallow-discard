using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public class CaseDbByMongo : ICaseDbForManage {
        private readonly IMongoCollection<Case> Db;

        public CaseDbByMongo(MongoDbContext mongo) {
            this.Db = mongo.Cases;
        }

        public IPagedList<Case> Index(
            CaseStatus status = CaseStatus.All,
            string userId = null,
            string articleId = null,
            string query = null,
            SortPattern pattern = SortPattern.Newest,
            int page = 1,
            int page_size = 30
        ) {
            var cases = Db.AsQueryable();

            if (status != CaseStatus.All)
                cases = cases.Where(d => d.Status == status);
            if (!string.IsNullOrEmpty(userId))
                cases = cases.Where(d => d.UserId == userId);
            if (!string.IsNullOrEmpty(articleId))
                cases = cases.Where(d => d.ArticleId == articleId);

            if (!string.IsNullOrEmpty(query))
                if (query.Length == 24)
                    cases = cases.Where(d => query == d.Id);
                else
                    cases = cases.Where(d => d.UserId == query || d.Words == query || d.ArticleTitle == query);


            cases = pattern.CaseOrderBy()(cases);

            return cases.ToPagedList(page, page_size);
        }

        public Case Get(string id) {
            return Db.AsQueryable().Where(d => d.Id == id).SingleOrDefault();
        }

        public Case Create(Case model, out string failure) {
            if (!EntityValidator.TryValidate(model, null, out failure))
                return null;
            model.Create();
            Db.InsertOne(model);
            return model;
        }

        #region ForManage
        public Case Update(Case model, out string failure) {
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
