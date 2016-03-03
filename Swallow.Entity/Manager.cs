using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Swallow.Entity {
    public class Manager {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [DisplayName("用户名")]
        public string Name { get; set; }

        public IList<string> Roles { get; set; }

        [Required]
        public string Password { get; set; } //Encrypted

        public static string HashPassword(string value) {
            return Encrypt.EncryptUserPassword(value);
        }

        public DateTime CreateTime { get; set; }
        public virtual void Creat() {
            CreateTime = DateTime.Now;
            Roles = new List<string> { "default" };
        }

        public DateTime DateLastLogin { get; set; }
    }
}
