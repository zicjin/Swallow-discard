using PagedList;
using Swallow.Entity;

namespace Swallow.Core {
    public interface IArticleDbForManage {
        IPagedList<Article> Index(
            ArticleStatus status = ArticleStatus.All, 
            ArticleType type = ArticleType.All, 
            ArticleVector vector = ArticleVector.All,
            string query = null,
            SortPattern pattern = SortPattern.Newest, 
            int page = 1,
            int page_size = 30
        );
        Article Get(string id);
        Article Create(Article model, out string failure);
        Article Update(Article model, out string failure);
        void Delete(string id, out string failure);
    }
}
