using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public interface IUserDbForManage {
        IPagedList<User> Index(UserStatus status = UserStatus.Normal, SortPattern pattern = SortPattern.Newest, string query = null, int page = 1, int page_size = 30);
        User Get(string id);
        User Update(User model, out string failure);
        void Delete(string id, out string failure);
    }
}