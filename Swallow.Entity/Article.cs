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
        All = 91
    }

    public enum ArticleType {
        Unknow = 0,
        Fiction = 1,
        Realism = 2,
        Video = 3,
        Voice = 4,
        Conversation = 5,
        All = 11
    }

    public enum ArticleVector {
        Text = 0,
        Pdf = 1,
        Web = 2,
        Markdown = 3,
        All = 11
    }

    public class Article {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public ArticleType Type { get; set; }

        public ArticleStatus Status { get; set; }
        public bool CouldShow() {
            return (Status != ArticleStatus.Freeze && Status != ArticleStatus.Delete);
        }

        public ArticleVector Vector { get; set; }
        public string VectorId { get; set; } // 载体

        public string NodeId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; } // 主动冗余

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "加入日期")]
        public DateTime CreateTime { get; set; }
        public virtual void Create() {
            CreateTime = DateTime.Now;
            Status = ArticleStatus.Unapproved;
        }

        public string Title { get; set; }
        public string Introduction { get; set; }

        public int Length { get; set; } // 长度（单位字）

        [Range(0, 10, ErrorMessage = "需要大于等于0，小于10")]
        public int DifficultyRank { get; set; } // 1-10 易到难
        [Range(0, 99, ErrorMessage = "需要大于等于0，小于100")]
        public int RedditRank { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "需要大于等于0")]
        public int LikeCount { get; set; }
    }
}
