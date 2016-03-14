using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Swallow.Entity;
using System;
using PagedList;
using System.Threading.Tasks;

namespace Swallow.Core {
    public class UserDbByMongo : IUserDbForManage {
        private readonly IMongoCollection<User> Db;

        public UserDbByMongo(CoreDbContext db) {
            this.Db = db.Users;
        }

        public IPagedList<User> Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, string query = null, int page = 1, int page_size = 30) {
            var users = Db.AsQueryable().Where(d =>
                status != UserStatus.All? d.Status == status: true
                && (string.IsNullOrEmpty(query) || query == d.Phone || query == d.Name)
            );

            users = pattern.GetOrderBy()(users);

            return users.ToPagedList(page, page_size);
        }

        public User Get(string id) {
            return Db.AsQueryable().Where(d => d.Id == id).SingleOrDefault();
        }

        #region ForManage
        public User Update(User model, out string failure) {
            if (!EntityValidator.TryValidate(model, "Password,CreatDate", out failure))
                return null;

            User user = Db.AsQueryable().Where(d => d.Id == model.Id).SingleOrDefault();
            if (user == null) {
                failure = "user不存在";
                return null;
            }

            if (Db.AsQueryable().Where(d => d.Id != model.Id && d.Phone == model.Phone).Any()) {
                failure = "Phone已被使用";
                return null;
            }
            
            model.Password = user.Password;
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
