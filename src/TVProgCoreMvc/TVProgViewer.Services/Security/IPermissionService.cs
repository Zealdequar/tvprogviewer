using System.Collections.Generic;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// Permission service interface
    /// </summary>
    public partial interface IPermissionService
    {
        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync();

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        Task UpdatePermissionRecordAsync(PermissionRecord permission);

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        Task InstallPermissionsAsync(IPermissionProvider permissionProvider);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync(PermissionRecord permission, User user);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, User user);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync(string permissionRecordSystemName, int userRoleId);

        /// <summary>
        /// Gets a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        Task<IList<PermissionRecordUserRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId);

        /// <summary>
        /// Delete a permission record-user role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="userRoleId">User role identifier</param>
        Task DeletePermissionRecordUserRoleMappingAsync(int permissionId, int userRoleId);

        /// <summary>
        /// Inserts a permission record-user role mapping
        /// </summary>
        /// <param name="permissionRecordUserRoleMapping">Permission record-user role mapping</param>
        Task InsertPermissionRecordUserRoleMappingAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping);
    }
}