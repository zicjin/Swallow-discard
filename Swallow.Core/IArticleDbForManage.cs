using PagedList;
using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Core {
    public interface IArticleDbForManage {
        IPagedList<Article> Index(
            ArticleStatus status = ArticleStatus.Normal, 
            ArticleType type = ArticleType.All, 
            ArticleVector vector = ArticleVector.All,
            SortPattern pattern = SortPattern.Newest, 
            string query = null, 
            int page = 1, 
            int page_size = 30
        );
        Article Get(string id);
        Article Create(Article model, out string failure);
        Article Update(Article model, out string failure);
        void Delete(string id, out string failure);
    }
}
