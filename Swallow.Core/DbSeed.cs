using Swallow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Core {
    public class DbSeed {
        public const string pwd_digest_z5656z = "jEPqc11pb5fOVRMrf8sN3w==";

        public static void MongoDB(MongoDbContext context) {
            var users = new List<User> {
                new User {
                    Phone = "18551090081",
                    Name ="金大侠",
                    Password = pwd_digest_z5656z,
                    Status = UserStatus.Normal
                },
                new User {
                    Phone = "18551090082",
                    Name ="金大侠",
                    Password = pwd_digest_z5656z,
                    Status = UserStatus.Normal
                },
                new User {
                    Phone = "18551090083",
                    Name ="vivian",
                    Password = pwd_digest_z5656z,
                    Status = UserStatus.Normal
                },
                new User {
                    Phone = "18551090084",
                    Name ="lotosbin",
                    Password = pwd_digest_z5656z,
                    Status = UserStatus.Normal
                },
                new User {
                    Phone = "18551090085",
                    Name ="老戚",
                    Password = pwd_digest_z5656z,
                    Status = UserStatus.Normal
                },
            };
            users.ForEach(s => {
                s.Create();
            });
            context.Users.InsertMany(users);
        }
    }
}
