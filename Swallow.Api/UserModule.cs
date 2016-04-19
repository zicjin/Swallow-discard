using Nancy;
using Swallow.Core;
using System;
using System.Collections.Generic;
using Nancy.ModelBinding;
using System.Linq;
using System.Threading.Tasks;
using Nancy.Authentication.Token;
using Nancy.Responses;
using Swallow.Entity;
using Swallow.Api.Security;
using Nancy.Security;
using Swallow.Service;

namespace Swallow.Api {
    public class UserModule : NancyModule {
        public UserModule(IUserDbForApi UserDb, ITokenizer tokenizer) :
            base("/users") {

            Post["/session"] = p => {
                var forms = this.Bind<UserLoginForms>();
                string failure;
                User user = UserDb.GetSession(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);

                user.RememberToken = tokenizer.Tokenize(new UserIdentity(user), Context);
                return user;
            };

            Get["/validation"] = p => {
                this.RequiresAuthentication();
                return HttpStatusCode.OK;
            };

            Get["/{userid:string}"] = p => {
                this.RequiresCurrentUser((string)p.userid);
                var userId = this.Context.CurrentUser.Claims.First();
                return UserDb.Get(userId);
            };

            // 验证新手机号
            Get["/{phone}/verify_phone"] = p => {
                string phone = p.phone;
                string failure;
                if (!UserDb.VerifyPhoneSendCode(phone, out failure, false))
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return HttpStatusCode.OK;
            };

            // 验证现有手机号
            Get["/{phone}/verify_exist_phone"] = p => {
                string phone = p.phone;
                string failure;
                if (!UserDb.VerifyPhoneSendCode(phone, out failure, true))
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return HttpStatusCode.OK;
            };

            // 注册
            Post["/create"] = p => {
                var forms = this.Bind<UserCreateForms>();

                string failure;
                User user = UserDb.Create(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);

                user.RememberToken = tokenizer.Tokenize(new UserIdentity(user), Context);
                return user;
            };

            // 找回密码
            Put["/found_password"] = p => {
                var forms = this.Bind<UserFoundPasswordForms>();
                string failure;
                var user = UserDb.FoundPassword(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return user;
            };

            // 更新名称
            Put["/name"] = p => {
                this.RequiresCurrentUser((string)p.userid);
                var forms = this.Bind<UserUpdateNameForms>();
                string failure;
                var user = UserDb.UpdateName(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return user;
            };

            // 更新密码
            Put["/password"] = p => {
                this.RequiresCurrentUser((string)p.userid);
                var forms = this.Bind<UserUpdatePasswordForms>();
                string failure;
                var user = UserDb.UpdatePassword(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return user;
            };

            // 更新手机号
            Get["/update_phone1"] = p => {
                this.RequiresCurrentUser((string)p.userid);
                var forms = this.Bind<UserUpdatePhoneForms1>();
                string failure;
                var user = UserDb.UpdatePhone1(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                return user;
            };
            Get["/update_phone2"] = p => {
                this.RequiresCurrentUser((string)p.userid);
                var forms = this.Bind<UserUpdatePhoneForms2>();
                string failure;
                var user = UserDb.UpdatePhone2(forms, out failure);
                if (user == null)
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, failure);
                user.RememberToken = tokenizer.Tokenize(new UserIdentity(user), Context);
                return user;
            };


        }

        private async Task<string> GetPathFromFile() {
            string path = null;
            if (Request.Files.Any()) {
                var file = Request.Files.First();
                var profileOssUpload = AliyunOssUploadBuild.BuildProfileOssUpload();
                var aliImage = await profileOssUpload.UploadAsync(file.Value);
                path = aliImage?.Url;
            }
            return path;
        }


    }
}