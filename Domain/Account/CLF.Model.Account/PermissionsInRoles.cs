using CLF.Model.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Model.Account
{
   public class PermissionsInRoles: BaseEntity
    {
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }

        public string RoleId { get; set; }

        public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
