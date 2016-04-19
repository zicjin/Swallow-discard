using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Swallow.Entity;
using System;
using PagedList;
using System.Threading.Tasks;
using MongoDB.Bson;
using Swallow.Service;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Swallow.Core {
    public class UserDbByMongo : IUserDbForManage, IUserDbForApi {
        private readonly IMongoCollection<User> Db;
        private readonly IEncryptorDecryptor EncryptorDecryptor;
        private readonly IVerifyCode VerifyCode;

        public UserDbByMongo(MongoDbContext mongo, IEncryptorDecryptor encryptorDecryptor, IVerifyCode verifyCode) {
            this.Db = mongo.Users;
            this.EncryptorDecryptor = encryptorDecryptor;
            this.VerifyCode = verifyCode;
        }

        public IPagedList<User> Index(
            UserStatus status = UserStatus.All, 
            string query = null, 
            SortPattern pattern = SortPattern.Newest, 
            int page = 1,
            int page_size = 30
        ) {
            var users = Db.AsQueryable();

            if (status != UserStatus.All)
                users = users.Where(d => d.Status == status);

            if (!string.IsNullOrEmpty(query)) // 不要查询ID http://stackoverflow.com/a/14315888
                users = users.Where(d => query == d.Phone || query == d.Name);

            users = pattern.UserOrderBy()(users);

            return users.ToPagedList(page, page_size);
        }

        public User Get(string id) {
            var user = Db.AsQueryable().Where(d => d.Id == id).SingleOrDefault();
            if (user == null)
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Warn, new string[] { id }, "User getById Null");
            return user;
        }

        #region ForApi
        public User Get(string id, out string failure) {
            failure = string.Empty;
            var user = Get(id);
            if (user == null) {
                failure = "用户不存在";
                return null;
            }
            return user;
        }

        public User GetByPhone(string phone, out string failure) {
            failure = string.Empty;
            var user = Db.AsQueryable().Where(d => d.Phone == phone).SingleOrDefault();
            if (user == null) {
                failure = "手机号不存在";
                return null;
            }
            return user;
        }

        public User GetSession(UserLoginForms forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            var user = GetByPhone(forms.Phone, out failure);
            if (user == null || user.Password != EncryptorDecryptor.Encrypt(forms.Password)) {
                failure = "手机号或密码错误";
                return null;
            }
            if (!user.CouldShow()) {
                failure = "账号被锁定";
                return null;
            }
            return user;
        }

        public bool ExistMobile(string phone) {
            return Db.AsQueryable().Where(d => d.Phone == phone).Any();
        }

        public bool VerifyPhoneSendCode(string phone, out string failure, bool exist = true) {
            failure = string.Empty;
            if (!Regex.IsMatch(phone, User.PhoneRegularExpression)) {
                failure = "手机号码格式不正确";
                return false;
            }
            if (exist && GetByPhone(phone, out failure) == null) {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Warn, new string[] { phone }, "Unexpected NoExist Phone");
                return false;
            }
            try {
                if (!VerifyCode.SendCode(phone)) {
                    failure = "验证码发送失败，请稍候重试";
                    return false;
                }
            } catch (VerifyCodeException e) {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Error, new string[] { phone }, e.ToJson());
            }

            return true;
        }

        // 安全警告：两步表单需要联合UserStatus.Verification验证
        private bool VerifyPhone(string phone, string code, out string failure, bool exist = true) {
            failure = string.Empty;
            try {
                if (!VerifyCode.Verify(phone, code)) {
                    failure = "验证码不正确";
                    return false;
                }
            } catch (Exception e) {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Error, new string[] { phone }, e.ToJson());
            }

            if (exist) {
                var user = GetByPhone(phone, out failure);
                if (user == null) {
                    LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Warn, new string[] { phone }, "Unexpected NoExist Phone");
                    return false;
                }
                user.Status = UserStatus.Verification;
                ReplaceOneResult result = Db.ReplaceOne(
                    d => d.Id == user.Id,
                    user,
                    new UpdateOptions { IsUpsert = true });
                if (!result.IsAcknowledged)
                    return false;
            }

            return true;
        }

        public User Create(UserCreateForms forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            if (ExistMobile(forms.Phone)) {
                failure = "电话号码已被注册";
                return null;
            }

            if(VerifyPhone(forms.Phone, forms.VerifyCode, out failure, false))
                return null;

            User newUser = new User {
                Phone = forms.Phone,
                Name = forms.Name,
                Password = EncryptorDecryptor.Encrypt(forms.Password)
            };
            newUser.Create();
            Db.InsertOne(newUser);
            return newUser;
        }

        public User ReportOne(string userId, string message, out string failure) {
            // Todo: 需要一个feedback数据表
            failure = string.Empty;
            var user = Get(userId, out failure);
            if (user == null)
                return null;
            user.Status = UserStatus.Reported;
            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == user.Id,
                user,
                new UpdateOptions { IsUpsert = true });

            if (!result.IsAcknowledged)
                return null;
            return user;
        }

        public User FoundPassword(UserFoundPasswordForms forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;

            var user = GetByPhone(forms.Phone, out failure);
            if (user == null)
                return null;

            if (user.CouldManageSelf()) {
                failure = "用户权限不足";
                return null;
            }

            if (VerifyPhone(forms.Phone, forms.VerifyCode, out failure, true))
                return null;

            user.Password = EncryptorDecryptor.Encrypt(forms.NewPassword);
            user.Status = UserStatus.Normal;
            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == user.Id,
                user,
                new UpdateOptions { IsUpsert = true });

            if (!result.IsAcknowledged)
                return null;
            return user;
        }

        public User UpdateName(UserUpdateNameForms forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            var user = Get(forms.Id, out failure);
            if (user == null)
                return null;

            user.Name = forms.Name;
            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == user.Id,
                user,
                new UpdateOptions { IsUpsert = true });

            if (result.IsAcknowledged)
                return user;
            return null;
        }

        public User UpdatePassword(UserUpdatePasswordForms forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            var user = Get(forms.Id, out failure);
            if (user == null)
                return null;

            if (user.Password != EncryptorDecryptor.Encrypt(forms.OldPassword)) {
                failure = "当前密码不正确";
                return null;
            }

            user.Password = EncryptorDecryptor.Encrypt(forms.NewPassword);
            ReplaceOneResult result = Db.ReplaceOne(
               d => d.Id == user.Id,
               user,
               new UpdateOptions { IsUpsert = true });

            if (result.IsAcknowledged)
                return user;
            return null;
        }

        public User UpdatePhone1(UserUpdatePhoneForms1 forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            User user = Get(forms.Id, out failure);
            if (user == null)
                return null;

            if (user.Password == EncryptorDecryptor.Encrypt(forms.Password)) {
                failure = "密码错误";
                return null;
            }

            if (VerifyPhone(user.Phone, forms.VerifyCode, out failure, true))
                return null;

            return user;
        }

        public User UpdatePhone2(UserUpdatePhoneForms2 forms, out string failure) {
            if (!EntityValidator.TryValidate(forms, null, out failure))
                return null;
            User user = Get(forms.Id, out failure);
            if (user == null)
                return null;

            if (user.Status != UserStatus.Verification) {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Core, LogType.Fatal, new string[] { forms.Id }, "Unexpected UserStatus.Verification");
                return null;
            }

            if (VerifyPhone(user.Phone, forms.VerifyCode, out failure, true))
                return null;

            user.Phone = forms.NewPhone;
            user.Status = UserStatus.Normal;
            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == user.Id,
                user,
                new UpdateOptions { IsUpsert = true });

            if (result.IsAcknowledged)
                return user;
            return null;
        }
        #endregion

        #region ForManage
        public User Create(User model, out string failure) {
            if (!EntityValidator.TryValidate(model, "Password,CreatDate", out failure))
                return null;

            if (Db.AsQueryable().Where(d => d.Id != model.Id && d.Phone == model.Phone).Any()) {
                failure = "Phone已被使用";
                return null;
            }

            model.Password = EncryptorDecryptor.Encrypt(model.Password);
            model.Create();
            Db.InsertOne(model);
            return model;
        }

        public User Update(User model, out string failure) {
            if (!EntityValidator.TryValidate(model, "Password,CreatDate", out failure))
                return null;

            User user = Db.AsQueryable().Where(d => d.Id == model.Id).SingleOrDefault();
            if (user == null) {
                failure = "user不存在";
                return null;
            }

            if (Db.AsQueryable().Where(d => d.Id != model.Id && d.Phone == model.Phone).Any()) {
                failure = "Phone已被使用";
                return null;
            }

            if (model.Status == UserStatus.All) {
                failure = "status is fail";
                return null;
            }
            model.Password = user.Password;
            ReplaceOneResult result = Db.ReplaceOne(
                d => d.Id == model.Id,
                model,
                new UpdateOptions { IsUpsert = true });

            if (result.IsAcknowledged)
                return model;
            return null;
        }

        public void Delete(string id, out string failure) {
            failure = string.Empty;
            DeleteResult result = Db.DeleteOne(
                d => d.Id == id
            );
            if (result.IsAcknowledged) {
                failure = "删除失败";
                return;
            }
            return;
        }
        #endregion
    }
}
