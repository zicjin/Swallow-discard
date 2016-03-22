using Nancy.Security;
using Swallow.Entity;
using System.Collections.Generic;

namespace Swallow.Api.Security {
    public class UserIdentity : IUserIdentity {
        public UserIdentity(User user) {
            UserName = user.Phone;
            Claims = new List<string> { user.Id.ToString() };
        }
        public string UserName { get; }
        public IEnumerable<string> Claims { get; }
    }
}