using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class PhraseEnCn {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Head { get; set; }
        public string Explain { get; set; }

        public IList<string> Examples { get; set; } // 列句

        // 外链 Prestige词典（朗道英汉词典）、自译词典（英英WordNet）
        // 外链 维基百科En、Google
    }
}
