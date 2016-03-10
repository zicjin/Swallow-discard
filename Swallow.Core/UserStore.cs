using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Swallow.Entity;
using System;
using PagedList;

namespace Swallow.Core {
    public class UserDb : IUserDb {
        private readonly IMongoCollection<User> Db;

        public UserDb(CoreDbContext db) {
            this.Db = db.Users;
        }

        public IPagedList<User> Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, string query = null, int page = 1, int page_size = 30) {
            var users = Db.AsQueryable().Where(d => 
                d.Status == status
                && (string.IsNullOrEmpty(query) || query == d.Phone || query == d.Name)
            );

            users = pattern.GetOrderBy()(users);

            return users.ToPagedList(page, page_size);
        }
    }
}
