using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class Node {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        public int PopularRank { get; set; }
    }
}
