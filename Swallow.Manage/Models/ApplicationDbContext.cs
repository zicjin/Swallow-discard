using AspNet.Identity3.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Swallow.Manage.Models {
    public class ApplicationDbContext : MongoIdentityContext<ApplicationUser, IdentityRole> {
        public ApplicationDbContext()
            : base() {
            string connectionString = "mongodb://localhost/SwallowManage";

            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<ApplicationUser>("users");
            this.Roles = database.GetCollection<IdentityRole>("roles");
        }
    }
}
