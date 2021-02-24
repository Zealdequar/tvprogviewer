using System.Collections.Generic;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// ACL service interface
    /// </summary>
    public partial interface IAclService
    {
        /// <summary>
        /// Get an expression predicate to apply the ACL
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="userRoleIds">Identifiers of user's roles</param>
        /// <returns>Lambda expression</returns>
        Expression<Func<TEntity, bool>> ApplyAcl<TEntity>(int[] userRoleIds) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        Task DeleteAclRecordAsync(AclRecord aclRecord);

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="userRoleId">User role id</param>
        Task InsertAclRecordAsync<TEntity>(TEntity entity, int userRoleId) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Get a value indicating whether any ACL records exist for entity type are related to user roles
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="userRoleIds">User's role identifiers</param>
        /// <returns>True if exist; otherwise false</returns>
        Task<bool> IsEntityAclMappingExistAsync<TEntity>(int[] userRoleIds) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>User role identifiers</returns>
        Task<int[]> GetUserRoleIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : BaseEntity, IAclSupported;
    }
}