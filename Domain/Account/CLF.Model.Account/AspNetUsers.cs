using CLF.Model.Account.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CLF.Model.Account
{
    [Table(Tables.AspNetUsers)]
    public class AspNetUsers : IdentityUser
    {
        public AspNetUsers()
        {
            IsDeleted = false;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        [MaxLength(250)]
        public string DisplayName { get; set; }

        [MaxLength(250)]
        public string OpenId { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [Display(Name = "创建人")]
        public string CreatedBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "修改人")]
        public string ModifiedBy { get; set; }

        [Display(Name = "修改时间")]
        public DateTime ModifiedDate { get; set; }
    }
}
