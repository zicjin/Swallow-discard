using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public enum CaseStatus {
        All = 0,
        Normal = 1,
        Done = 10,
        Delete = 90,
    }

    public class Case {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public CaseStatus Status { get; set; }

        public string UserId { get; set; }
        public string ArticleId { get; set; }
        public string ArticleTitle { get; set; } // 主动冗余

        public string Words { get; set; }

        public string Context { get; set; }
        public int Rank { get; set; } // 记忆曲线
        public DateTime CreateTime { get; set; }

        public virtual void Create() {
            CreateTime = DateTime.Now;
            Status = CaseStatus.Normal;
        }

        // public string Postition { get; set; } // Todo: 与全文索引技术有关，待议

        // public string TransForContext { get; set; } // Todo: Maybe
    }
}
