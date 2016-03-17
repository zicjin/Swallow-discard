using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public enum UserStatus {
        All = 0,
        Unapproved = 1,
        Normal = 2,
        Charger = 3,
        Reported = 10,
        Freeze = 11,
        Admin = 80,
        Delete = 90,
    }

    public class User {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public UserStatus Status { get; set; }
        public bool CouldShow() {
            return (Status != UserStatus.Freeze && Status != UserStatus.Delete);
        }

        [Required]
        [DisplayName("用户名")]
        public string Name { get; set; }

        public const string PhoneRegularExpression = @"^1[3,4,5,7,8]\d{9}$";
        [Required]
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(PhoneRegularExpression, ErrorMessage = "手机号码格式不正确")]
        [StringLength(11)] // EF添加索引要有stringlength http://stackoverflow.com/a/22737780
        public string Phone { get; set; }

        public string TokenIdentity { get; set; } //用于生成权限票据ticket

        [Required]
        public string Password { get; set; } //Encrypted
        public static string HashPassword(string value) {
            return Encrypt.EncryptUserPassword(value);
        }

        public IEnumerable<string> Roles { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "加入日期")]
        public DateTime CreateTime { get; set; }
        public virtual void Create() {
            CreateTime = DateTime.Now;
            Status = UserStatus.Unapproved;
        }

        [Display(Name = "最近使用日期")]
        public DateTime LastLoginTime { get; set; }

        public int Rank { get; set; } // article popular
        public int CaseCount { get; set; } // 主动冗余
    }
}
