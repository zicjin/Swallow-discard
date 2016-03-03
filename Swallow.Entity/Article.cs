using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public enum ArticleStatus {
        Unapproved = 0,
        Normal = 1,
        Reported = 10,
        Freeze = 11,
        Delete = 90,
    }

    public enum ArticleType {
        Unknow = 0,
        Fiction = 1,
        Realism = 2,
        Video = 3,
        Voice = 4,
        Conversation = 5,
    }

    public enum ArticleVector {
        Text = 0,
        Pdf = 1,
        Web = 2,
        Markdown = 3,
    }

    public class Article {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public ArticleType Type { get; set; }

        public ArticleStatus Status { get; set; }
        public bool CouldShow() {
            return (Status != ArticleStatus.Freeze && Status != ArticleStatus.Delete);
        }

        public string NodeId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; } // 主动冗余

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "加入日期")]
        public DateTime CreateTime { get; set; }
        public virtual void Creat() {
            CreateTime = DateTime.Now;
            Status = ArticleStatus.Unapproved;
        }

        public ArticleVector Vector { get; set; }
        public string VectorId { get; set; } // 载体

        public string Title { get; set; }
        public string Introduction { get; set; }

        public int Length { get; set; } // 长度（单位字）

        public int DifficultyRank { get; set; } // 1-10 易到难
        public int RedditRank { get; set; }
        public int LikeCount { get; set; }
    }
}
