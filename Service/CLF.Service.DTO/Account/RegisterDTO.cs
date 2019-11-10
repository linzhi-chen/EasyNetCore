using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CLF.Service.DTO.Account
{
   public class RegisterDTO
    {
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "邮箱格式不正确")]
        [MaxLength(50, ErrorMessage = "{0}最大长度为{1}字符")]
        [Remote("ExistEmail", "Account", ErrorMessage = "邮箱已经存在", HttpMethod = "post")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"1[0-9]{10}$", ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机")]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [StringLength(16, ErrorMessage = "密码长度必须在{2}位与{1}位之间。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [StringLength(16, ErrorMessage = "密码长度必须在{2}位与{1}位之间。", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密码和确认密码不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
