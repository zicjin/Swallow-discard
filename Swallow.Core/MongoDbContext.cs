using MongoDB.Driver;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swallow.Core {
    public class MongoDbContext {
        public MongoDbContext(string connection) {
            var database = new MongoClient(connection).GetDatabase("Swallow");
            this.Users = database.GetCollection<User>("users");
            this.Articles = database.GetCollection<Article>("articles");
            this.Cases = database.GetCollection<Case>("cases");
        }

        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<Article> Articles { get; set; }
        public IMongoCollection<Case> Cases { get; set; }
    }
}
