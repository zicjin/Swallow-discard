using MongoDB.Driver.Linq;
using System;
using System.Linq;

namespace Swallow.Entity {
    public enum SortPattern {
        Newest = 0,
        Max = 1,
        Rank = 2,
        Quality = 3
    }

    public static class OrderByPattern {
        public static Func<IMongoQueryable<User>, IOrderedMongoQueryable<User>> UserOrderBy(this SortPattern pattern) {
            Func<IMongoQueryable<User>, IOrderedMongoQueryable<User>> _orderby;
            switch (pattern) {
                case SortPattern.Rank:
                    _orderby = (a => a.OrderByDescending(d => d.Rank));
                    break;
                case SortPattern.Max:
                    _orderby = (a => a.OrderByDescending(d => d.CaseCount));
                    break;
                default: // Newest
                    _orderby = (a => a.OrderByDescending(d => d.CreateTime));
                    break;
            }
            return _orderby;
        }

        public static Func<IMongoQueryable<Article>, IOrderedMongoQueryable<Article>> ArticleOrderBy(this SortPattern pattern) {
            Func<IMongoQueryable<Article>, IOrderedMongoQueryable<Article>> _orderby;
            switch (pattern) {
                case SortPattern.Rank:
                    _orderby = (a => a.OrderByDescending(d => d.RedditRank));
                    break;
                case SortPattern.Max:
                    _orderby = (a => a.OrderByDescending(d => d.LikeCount));
                    break;
                case SortPattern.Quality:
                    _orderby = (a => a.OrderByDescending(d => d.DifficultyRank));
                    break;
                default: // Newest
                    _orderby = (a => a.OrderByDescending(d => d.CreateTime));
                    break;
            }
            return _orderby;
        }

        public static Func<IMongoQueryable<Case>, IOrderedMongoQueryable<Case>> CaseOrderBy(this SortPattern pattern) {
            Func<IMongoQueryable<Case>, IOrderedMongoQueryable<Case>> _orderby;
            switch (pattern) {
                case SortPattern.Rank:
                    _orderby = (a => a.OrderByDescending(d => d.Rank));
                    break;
                default: // Newest
                    _orderby = (a => a.OrderByDescending(d => d.CreateTime));
                    break;
            }
            return _orderby;
        }
    }

}
