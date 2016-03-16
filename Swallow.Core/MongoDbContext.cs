using MongoDB.Driver;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swallow.Core {
    public class MongoDbContext {
        public MongoDbContext(string connectionString) {
            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<User>("users");
            this.Articles = database.GetCollection<Article>("articles");
        }

        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<Article> Articles { get; set; }
    }
}
