using MongoDB.Driver;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swallow.Manage.Models {
    public class AppDbContext {
        public AppDbContext(string connectionString): base() {
            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<User>("users");
        }

        public IMongoCollection<User> Users { get; set; }
    }
}
