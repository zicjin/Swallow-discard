using MongoDB.Driver;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swallow.Core {
    public class CoreDbContext {
        public CoreDbContext(string connectionString) {
            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<User>("users");
        }

        public IMongoCollection<User> Users { get; set; }
    }
}
