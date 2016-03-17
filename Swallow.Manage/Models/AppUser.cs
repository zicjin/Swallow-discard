using AspNet.Identity3.MongoDB;
using System;

namespace Swallow.Manage.Models {
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser {
        public const string Normal = "normal";
        public const string Cteator = "cteator";
        public const string Admin = "admin";
    }
}
