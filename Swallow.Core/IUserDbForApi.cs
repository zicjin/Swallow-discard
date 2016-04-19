using PagedList;
using Swallow.Entity;
using System.ComponentModel.DataAnnotations;

namespace Swallow.Core {
    public interface IUserDbForApi {
        User Get(string id);
        User GetByPhone(string phone, out string failure);
        User GetSession(UserLoginForms forms, out string failure);
        bool ExistMobile(string phone);

        bool VerifyPhoneSendCode(string phone, out string failure, bool exist = true);

        User Create(UserCreateForms forms, out string failure);

        User ReportOne(string userId, string message, out string failure);
        User FoundPassword(UserFoundPasswordForms forms, out string failure);
        User UpdateName(UserUpdateNameForms forms, out string failure);
        User UpdatePassword(UserUpdatePasswordForms forms, out string failure);
        User UpdatePhone1(UserUpdatePhoneForms1 forms, out string failure);
        User UpdatePhone2(UserUpdatePhoneForms2 forms, out string failure);
    }

    public class UserLoginForms {
        [Required(ErrorMessage = "请输入您的手机号"), Display(Name = "手机号")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "请输入您的密码")]
        public string Password { get; set; }
    }

    public class UserCreateForms {
        [RegularExpression(User.PhoneRegularExpression, ErrorMessage = "手机号码格式不正确")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "请输入验证码"), Display(Name = "验证码")]
        public string VerifyCode { get; set; }
        [Required(ErrorMessage = "请输入您的昵称")]
        [StringLength(10, ErrorMessage = "您的昵称不能大于10个字符")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入您的密码")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少6位")]
        public string Password { get; set; }
    }

    public class UserUpdateNameForms {
        public string Id { get; set; }
        [Required(ErrorMessage = "请输入您的新昵称")]
        [Display(Name = "昵称")]
        [StringLength(10, ErrorMessage = "您的昵称不能大于10个字符")]
        public string Name { get; set; }
    }

    public class UserFoundPasswordForms {
        [Required(ErrorMessage = "请输入验证码"), Display(Name = "验证码")]
        public string VerifyCode { get; set; }

        [Required(ErrorMessage = "请输入您的手机号"), Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(User.PhoneRegularExpression, ErrorMessage = "手机号码格式不正确")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "请输入您的密码"), Display(Name = "密码")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少6位")]
        public string NewPassword { get; set; }
    }

    public class UserUpdatePasswordForms {
        [Required(ErrorMessage = "请提供用户标识")]
        public string Id { get; set; }
        [Required(ErrorMessage = "请输入您的当前密码")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "请输入您的新密码")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少6位")]
        public string NewPassword { get; set; }
    }

    public class UserUpdatePhoneForms1 {
        public string Id { get; set; }
        public string VerifyCode { get; set; }

        [Required(ErrorMessage = "请输入密码"), Display(Name = "密码")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少6位")]
        public string Password { get; set; }
    }

    public class UserUpdatePhoneForms2 {
        public string Id { get; set; }

        [Required(ErrorMessage = "请输入新的手机号")]
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(User.PhoneRegularExpression, ErrorMessage = "手机号码格式不正确")]
        public string NewPhone { get; set; }

        public string VerifyCode { get; set; }
    }

}