using CLF.Model.Account;
using CLF.Service.DTO.Account;
using CLF.Service.DTO.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Service.Account
{
    public interface IAccountService
    {
        #region 权限
        PermissionDTO GetPermissionById(int id);
        PaginatedBaseDTO<PermissionDTO> FindPagenatedListWithCount(int pageIndex, int pageSize,string controllerName, string actionName);
        bool AddPermission(PermissionDTO permission);
        bool ModifyPermission(PermissionDTO permission);
        bool DeletePermissions(List<int> ids);
        List<PermissionDTO> GetPermissionsBy(string controllerName, string actionName);
        #endregion

        #region 登陆注册
        Task<KeyValuePair<IdentityResult, AspNetUsers>> CreateUserAsync(RegisterDTO model);

        #endregion
    }
}
