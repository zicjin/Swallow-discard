using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class Words {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public eDictionaryType Type { get; set; }

        public string Head { get; set; }

        public string Soundmark { get; set; }
        public string SoundmarkUK { get; set; }

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


        public string Plural { get; set; } // 复数
        public string Singular { get; set; } // 第三人称单数
        public string ComparativeDegree { get; set; } // 比较级
        public string SuperlativeDegree { get; set; }
        public string PastTense { get; set; } // 过去式
        public string PastParticiple { get; set; } // 过去分词
        public string PresentParticiple { get; set; } // 现在分词

        public IList<string> Synonym { get; set; } // 同义
        public IList<string> Antonym { get; set; }

        public string Root { get; set; } // 词根
        public IList<string> SynonymRootWord  { get; set; } // 同词根

        public IList<string> Cases { get; set; } // 词组
        public IList<string> Examples { get; set; } // 列句

        // Oxford Collocations Dictionary for learners of English http://collocationdictionary.freedicts.com/
        public IList<string> Collocations { get; set; }

        // 外链 Prestige词典、关联词典（如英汉关联了英英）、百科
    }
}
