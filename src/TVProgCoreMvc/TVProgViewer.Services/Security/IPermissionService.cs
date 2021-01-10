using System.Collections.Generic;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// Permission service interface
    /// </summary>
    public partial interface IPermissionService
    {
        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        void DeletePermissionRecord(PermissionRecord permission);

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        PermissionRecord GetPermissionRecordById(int permissionId);

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        PermissionRecord GetPermissionRecordBySystemName(string systemName);

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        IList<PermissionRecord> GetAllPermissionRecords();

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        void InsertPermissionRecord(PermissionRecord permission);

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        void UpdatePermissionRecord(PermissionRecord permission);

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        void InstallPermissions(IPermissionProvider permissionProvider);

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        void UninstallPermissions(IPermissionProvider permissionProvider);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(PermissionRecord permission);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="User">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(PermissionRecord permission, User User);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="User">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName, User User);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName, int UserRoleId);

        /// <summary>
        /// Gets a permission record-User role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        IList<PermissionRecordUserRoleMapping> GetMappingByPermissionRecordId(int permissionId);

        /// <summary>
        /// Delete a permission record-User role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="UserRoleId">User role identifier</param>
        void DeletePermissionRecordUserRoleMapping(int permissionId, int UserRoleId);

        /// <summary>
        /// Inserts a permission record-User role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-User role mapping</param>
        void InsertPermissionRecordUserRoleMapping(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping);
    }
}