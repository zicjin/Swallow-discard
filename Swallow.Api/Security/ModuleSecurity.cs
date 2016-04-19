using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.Security;

namespace Swallow.Api.Security {
    public static class ModuleSecurity {
        public static void RequiresCurrentUser(this NancyModule module, string userId) {
            module.RequiresAuthentication();
            var loginId = module.Context.CurrentUser.Claims.First();
            if (userId != loginId) {
                throw new RouteExecutionEarlyExitException(new TextResponse(HttpStatusCode.Forbidden));
            }
        }
    }
}