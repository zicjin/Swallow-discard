using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Swallow.Entity;
using System;
using PagedList;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Swallow.Core {
    public class UserDbByMongo : IUserDbForManage {
        private readonly IMongoCollection<User> Db;

        public UserDbByMongo(MongoDbContext mongo) {
            this.Db = mongo.Users;
        }

        public IPagedList<User> Index(
            UserStatus status = UserStatus.All, 
            string query = null, 
            SortPattern pattern = SortPattern.Newest, 
            int page = 1,
            int page_size = 30
        ) {
            var users = Db.AsQueryable();

            if (status != UserStatus.All)
                users = users.Where(d => d.Status == status);

            if (!string.IsNullOrEmpty(query)) // 不要查询ID http://stackoverflow.com/a/14315888
                users = users.Where(d => query == d.Phone || query == d.Name);

            users = pattern.UserOrderBy()(users);

            return users.ToPagedList(page, page_size);
        }

        public User Get(string id) {
            return Db.AsQueryable().Where(d => d.Id == id).SingleOrDefault();
        }

        #region ForManage
        public User Create(User model, out string failure) {
            if (!EntityValidator.TryValidate(model, "Password,CreatDate", out failure))
                return null;

            if (Db.AsQueryable().Where(d => d.Id != model.Id && d.Phone == model.Phone).Any()) {
                failure = "Phone已被使用";
                return null;
            }

            model.Password = User.HashPassword(model.Password);
            model.Create();
            Db.InsertOne(model);
            return model;
        }

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
