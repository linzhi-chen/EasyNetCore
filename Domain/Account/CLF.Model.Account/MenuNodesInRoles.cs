using CLF.Model.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Model.Account
{
  public  class MenuNodesInRoles:BaseEntity
    {
        public int MenuNodeId { get; set; }

        public virtual  MenuNode MenuNode { get; set; }

        public string RoleId { get; set; }

        public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
