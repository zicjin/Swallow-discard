using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public interface ICaseDbForManage {
        IPagedList<Case> Index(
            CaseStatus status = CaseStatus.All,
            string userId = null,
            string articleId = null,
            string query = null,
            SortPattern pattern = SortPattern.Newest,
            int page = 1, 
            int page_size = 30
        );
        Case Get(string id);
        Case Create(Case model, out string failure);
        Case Update(Case model, out string failure);
        void Delete(string id, out string failure);
    }
}
