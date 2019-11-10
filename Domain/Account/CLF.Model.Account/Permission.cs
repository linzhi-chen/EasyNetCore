using CLF.Model.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Model.Account
{
  public  class Permission: TreeNode<Permission>
    {
        public Permission()
        {
            this.PermissionsInRoles = new List<PermissionsInRoles>();
        }

        public string AreaName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string Description { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }

        public virtual IList<PermissionsInRoles> PermissionsInRoles { get; set; }

    }
}
