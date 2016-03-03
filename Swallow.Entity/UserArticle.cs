using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swallow.Entity {
    public class UserArticle {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string ArticleId { get; set; }

        public bool IsLike { get; set; }

        public int ReadProgress { get; set; } // percent
        public DateTime LastReadTime { get; set; }
    }
}
