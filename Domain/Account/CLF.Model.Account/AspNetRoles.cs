using CLF.Model.Account.Constants;
using CLF.Model.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace CLF.Model.Account
{
    [Table(Tables.AspNetRoles)]
    public class AspNetRoles : IdentityRole
    {
        public AspNetRoles()
        {
            IsDeleted = false;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;

            this.MenuNodesInRoles = new List<MenuNodesInRoles>();
            this.PermissionsInRoles = new List<PermissionsInRoles>();
        }
        [MaxLength(250)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

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

        public virtual IList<MenuNodesInRoles> MenuNodesInRoles { get; set; }
        public virtual IList<PermissionsInRoles> PermissionsInRoles { get; set; }
    }
}
 