using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public interface IUserDb {
        IPagedList<User> Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, string query = null, int page = 1, int page_size = 30);
    }
}