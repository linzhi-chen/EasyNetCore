using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using CLF.Common.Caching;
using CLF.DataAccess.Account.Repository;
using CLF.Model.Account;
using CLF.Service.Core.Events;
using CLF.Service.DTO.Account;
using CLF.Common.Extensions;
using CLF.Service.Core.Extensions;
using CLF.Common.Infrastructure.Mapper;
using CLF.Service.DTO.Core;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CLF.Common.Exceptions;
using CLF.Common.SecurityHelper;
using System.Linq;
using CLF.Domain.Core.EFRepository;

namespace CLF.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly PermissionRepository _permissionRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly RoleManager<AspNetRoles> _roleManager;
 
        public AccountService(PermissionRepository permissionRepository, IStaticCacheManager staticCacheManager, IEventPublisher eventPublisher
            , UserManager<AspNetUsers> userManager, RoleManager<AspNetRoles> roleManager)
        {
            this._permissionRepository = permissionRepository;
            this._staticCacheManager = staticCacheManager;
            this._eventPublisher = eventPublisher;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public bool AddPermission(PermissionDTO model)
        {
            var permission = model.Map<Permission>();
            _eventPublisher.EntityInserted(permission);//清缓存
            return _permissionRepository.Add(permission);
        }

        public bool ModifyPermission(PermissionDTO model)
        {
            var permission = _permissionRepository.FindById(model.Id);
            permission.Name = model.Name;
            permission.ActionName = model.ActionName;
            permission.ControllerName = model.ControllerName;
            permission.Description = model.Description;

            _eventPublisher.EntityUpdated(permission);//清缓存

            return _permissionRepository.Modify(permission);
        }

        public bool DeletePermissions(List<int> ids)
        {
            var permissions = _permissionRepository.Find(m => ids.Contains(m.Id) && !m.IsDeleted);
            _eventPublisher.EntityDeleted(permissions[0]);//清缓存
            return _permissionRepository.Remove(permissions) > 0;
        }

        public PaginatedBaseDTO<PermissionDTO> FindPagenatedListWithCount(int pageIndex, int pageSize, string controllerName,string actionName)
        {
            string searchValue = $"{controllerName}-{actionName}-{pageIndex}-{pageSize}";
            var cacheKey = string.Format( AccountServiceDefaults.GetPermissionListCacheKey,searchValue);

            return _staticCacheManager.Get(cacheKey, () =>
            {
                Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
                if (!string.IsNullOrEmpty(controllerName))
                {
                    predicate = predicate.And(m => m.ControllerName.Contains(controllerName));
                }
                if (!string.IsNullOrEmpty(actionName))
                {
                    predicate = predicate.And(m => m.ActionName.Contains(actionName));
                }
                var data = _permissionRepository.FindPagenatedListWithCount<Permission>(pageIndex/pageSize, pageSize, predicate, "CreatedDate", false);
                var count = data.Item1;
                var permissionList = data.Item2.Map<List<PermissionDTO>>();
                return new PaginatedBaseDTO<PermissionDTO>(pageIndex, pageSize, count, count, permissionList);
            });
        }

        public PermissionDTO GetPermissionById(int id)
        {
            var permission = _permissionRepository.FindById(id);
            return permission.Map<PermissionDTO>();
        }

        public List<PermissionDTO> GetPermissionsBy(string controllerName, string actionName)
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(controllerName))
            {
                predicate = predicate.And(m => m.ControllerName.Contains(controllerName));
            }
            if (!string.IsNullOrEmpty(actionName))
            {
                predicate = predicate.And(m => m.ActionName.Contains(actionName));
            }
            var result = _permissionRepository.LoadPermissionByFilter(predicate, "CreatedDate");
            return result.Map<List<PermissionDTO>>();
        }

        public async Task<KeyValuePair<IdentityResult, AspNetUsers>> CreateUserAsync(RegisterDTO model)
        {
            AspNetUsers aspNetUsers = new AspNetUsers
            {
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                Email = model.Email.Trim(),
                UserName = model.Email.Trim(),
                PhoneNumber = "180" + MD5Provider.Hash(model.Email.Trim()).ToString().Substring(0, 8)
            };
            var result = await _userManager.CreateAsync(aspNetUsers, model.Password);
            return new KeyValuePair<IdentityResult, AspNetUsers>(result, aspNetUsers);
        }
    }
}
