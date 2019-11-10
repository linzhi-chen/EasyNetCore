using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CLF.Service.DTO.Account
{
  public  class SignInDTO
    {
        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "邮箱或手机")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "密码长度必须在{2}位与{1}位之间。", MinimumLength = 6)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住用户名")]
        public bool RememberMe { get; set; }

        [Display(Name = "微信公众号")]
        public string OpenId { get; set; }

        //[Required(ErrorMessage = "{0}必填")]
        [StringLength(4, ErrorMessage = "请输入4位验证码")]
        [Display(Name = "验证码")]
        public string VerificationCode { get; set; }

        [Display(Name = "登录失败次数")]
        public int FailedLoginCount { get; set; }
    }
}
