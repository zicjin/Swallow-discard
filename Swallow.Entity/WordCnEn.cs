using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class WordCnEn {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Head { get; set; }

        public string Soundmark { get; set; }

        // Explain http://baike.baidu.com/view/377635.htm
        public string ExPrep { get; set; }
        public string ExPron { get; set; }
        public string ExArt { get; set; }
        public string ExN { get; set; }
        public string ExV { get; set; }
        public string ExVi { get; set; }
        public string ExVt { get; set; }
        public string ExConj { get; set; }
        public string ExAdj { get; set; }
        public string ExAdv { get; set; }
        public string ExNum { get; set; }
        public string ExInt { get; set; }

        public IList<string> Synonym { get; set; } // 同义
        public IList<string> Antonym { get; set; }

        public IList<string> Cases { get; set; } // 词组
        public IList<string> Examples { get; set; } // 列句

        // 外链 Prestige词典（新汉英大辞典）、自译词典（现代汉语大词典）
        // 外链 百度百科、Bing
    }
}
