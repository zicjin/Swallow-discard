using AspNet.Identity3.MongoDB;
using MongoDB.Driver;

namespace Swallow.Manage.Models {
    public class IdentityContext : MongoIdentityContext<AppUser, IdentityRole> {
        public IdentityContext(string connectionString): base() {
            var database = new MongoClient(connectionString).GetDatabase("SwallowManage");
            this.Users = database.GetCollection<AppUser>("app_users");
            this.Roles = database.GetCollection<IdentityRole>("app_roles");
        }
    }
}
