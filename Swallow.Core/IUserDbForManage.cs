using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public interface IUserDbForManage {
        IPagedList<User> Index(
            UserStatus status = UserStatus.All, 
            string query = null, 
            SortPattern pattern = SortPattern.Newest, 
            int page = 1,
            int page_size = 30
        );
        User Get(string id);
        User Create(User model, out string failure);
        User Update(User model, out string failure);
        void Delete(string id, out string failure);
    }
}