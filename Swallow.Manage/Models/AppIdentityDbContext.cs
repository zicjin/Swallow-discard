using AspNet.Identity3.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swallow.Manage.Models {
    public class AppIdentityDbContext : MongoIdentityContext<AppUser, IdentityRole> {
        public AppIdentityDbContext(string connectionString): base() {
            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<AppUser>("app_users");
            this.Roles = database.GetCollection<IdentityRole>("app_roles");
        }
    }
}
