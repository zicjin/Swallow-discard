using MongoDB.Driver.Linq;
using System;
using System.Linq;

namespace Swallow.Entity {
    public enum SortPattern {
        Newest = 0,
        Max = 1,
        Rank = 2
    }

    public static class OrderByPattern {
        public static Func<IMongoQueryable<User>, IOrderedMongoQueryable<User>> GetOrderBy(this SortPattern pattern) {
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
    }

}
