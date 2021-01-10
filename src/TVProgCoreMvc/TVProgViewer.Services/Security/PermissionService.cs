using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data;
using TVProgViewer.Services.Caching.CachingDefaults;
using TVProgViewer.Services.Caching.Extensions;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IRepository<PermissionRecordUserRoleMapping> _permissionRecordUserRoleMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PermissionService(IUserService UserService,
            ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordUserRoleMapping> permissionRecordUserRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _userService = UserService;
            _localizationService = localizationService;
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordUserRoleMappingRepository = permissionRecordUserRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by User role identifier
        /// </summary>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>Permissions</returns>
        protected virtual IList<PermissionRecord> GetPermissionRecordsByUserRoleId(int UserRoleId)
        {
            var key = TvProgSecurityCachingDefaults.PermissionsAllByUserRoleIdCacheKey.FillCacheKey(UserRoleId);

            var query = from pr in _permissionRecordRepository.Table
                join prcrm in _permissionRecordUserRoleMappingRepository.Table on pr.Id equals prcrm
                    .PermissionRecordId
                where prcrm.UserRoleId == UserRoleId
                orderby pr.Id
                select pr;

            return query.ToCachedList(key);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRecordRepository.Delete(permission);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionRecordRepository.ToCachedGetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = query.FirstOrDefault();
            return permissionRecord;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from pr in _permissionRecordRepository.Table
                        orderby pr.Name
                        select pr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRecordRepository.Insert(permission);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRecordRepository.Update(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            //default User role mappings
            var defaultPermissions = permissionProvider.GetDefaultPermissions().ToList();

            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 != null)
                    continue;

                //new permission (install it)
                permission1 = new PermissionRecord
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                    Category = permission.Category
                };

                //save new permission
                InsertPermissionRecord(permission1);

                foreach (var defaultPermission in defaultPermissions)
                {
                    var UserRole = _userService.GetUserRoleBySystemName(defaultPermission.systemRoleName);
                    if (UserRole == null)
                    {
                        //new role (save it)
                        UserRole = new UserRole
                        {
                            Name = defaultPermission.systemRoleName,
                            Active = true,
                            SystemName = defaultPermission.systemRoleName
                        };
                        _userService.InsertUserRole(UserRole);
                    }

                    var defaultMappingProvided = defaultPermission.permissions.Any(p => p.SystemName == permission1.SystemName);

                    if (!defaultMappingProvided)
                        continue;

                    InsertPermissionRecordUserRoleMapping(new PermissionRecordUserRoleMapping { UserRoleId = UserRole.Id, PermissionRecordId = permission1.Id });
                }

                //save localization
                _localizationService.SaveLocalizedPermissionName(permission1);
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                    continue;

                DeletePermissionRecord(permission1);

                //delete permission locales
                _localizationService.DeleteLocalizedPermissionName(permission1);
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="User">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, User User)
        {
            if (permission == null)
                return false;

            if (User == null)
                return false;

            //old implementation of Authorize method
            //var UserRoles = User.UserRoles.Where(cr => cr.Active);
            //foreach (var role in UserRoles)
            //    foreach (var permission1 in role.PermissionRecords)
            //        if (permission1.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
            //            return true;
            //return false;

            return Authorize(permission.SystemName, User);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            return Authorize(permissionRecordSystemName, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="User">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName, User user)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var userRoles = _userService.GetUserRoles(user);
            foreach (var role in userRoles)
                if (Authorize(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName, int userRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = TvProgSecurityCachingDefaults.PermissionsAllowedCacheKey.FillCacheKey(permissionRecordSystemName, userRoleId);
            return _staticCacheManager.Get(key, () =>
            {
                var permissions = GetPermissionRecordsByUserRoleId(userRoleId);
                foreach (var permission1 in permissions)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Gets a permission record-User role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        public virtual IList<PermissionRecordUserRoleMapping> GetMappingByPermissionRecordId(int permissionId)
        {
            var query = _permissionRecordUserRoleMappingRepository.Table;

            query = query.Where(x => x.PermissionRecordId == permissionId);

            return query.ToList();
        }

        /// <summary>
        /// Delete a permission record-User role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="UserRoleId">User role identifier</param>
        public virtual void DeletePermissionRecordUserRoleMapping(int permissionId, int UserRoleId)
        {
            var mapping = _permissionRecordUserRoleMappingRepository.Table.FirstOrDefault(prcm => prcm.UserRoleId == UserRoleId && prcm.PermissionRecordId == permissionId);

            if (mapping is null)
                throw new Exception(string.Empty);

            _permissionRecordUserRoleMappingRepository.Delete(mapping);
        }

        /// <summary>
        /// Inserts a permission record-User role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-User role mapping</param>
        public virtual void InsertPermissionRecordUserRoleMapping(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
        {
            if (permissionRecordUserRoleMapping is null)
                throw new ArgumentNullException(nameof(permissionRecordUserRoleMapping));

            _permissionRecordUserRoleMappingRepository.Insert(permissionRecordUserRoleMapping);
        }

        #endregion
    }
}