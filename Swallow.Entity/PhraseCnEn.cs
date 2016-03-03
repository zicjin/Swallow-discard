using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class PhraseCnEn {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Head { get; set; }
        public string Explain { get; set; }

        public IList<string> Examples { get; set; } // 列句

        // 外链 Prestige词典（维科汉英词典）、自译词典（现代汉语大词典）
        // 外链 百度百科、Bing
    }
}
