using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Swallow.Entity {
    public class OAuth {

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AccountId { get; set; }

        [MaxLength(30)]
        public string Provider { get; set; }
        [MaxLength(100)]
        public string ProviderUserId { get; set; }

        public int ExpireTime { get; set; }
        public string Token { get; set; }

        public string NickName { get; set; }
        public string HeadUrl { get; set; }

        public string ProfileUrl { get; set; }
    }
}
