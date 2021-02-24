using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;
using System.Threading.Tasks;
using TVProgViewer.Core.Caching;
using System.Linq.Expressions;

namespace TVProgViewer.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IUserService _userService;
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AclService(CatalogSettings catalogSettings,
            IUserService userService,
            IRepository<AclRecord> aclRecordRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _userService = userService;
            _aclRecordRepository = aclRecordRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        protected virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.InsertAsync(aclRecord);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get an expression predicate to apply the ACL
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="userRoleIds">Identifiers of user's roles</param>
        /// <returns>Lambda expression</returns>
        public virtual Expression<Func<TEntity, bool>> ApplyAcl<TEntity>(int[] userRoleIds) where TEntity : BaseEntity, IAclSupported
        {
            return (entity) =>
                (from acl in _aclRecordRepository.Table
                 where !entity.SubjectToAcl ||
                    (acl.EntityId == entity.Id && acl.EntityName == typeof(TEntity).Name && userRoleIds.Contains(acl.UserRoleId))
                 select acl.EntityId).Any();
        }

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
        {
            await _aclRecordRepository.DeleteAsync(aclRecord);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = await query.ToListAsync();

            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="userRoleId">User role id</param>
        public virtual async Task InsertAclRecordAsync<TEntity>(TEntity entity, int userRoleId) where TEntity : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (userRoleId == 0)
                throw new ArgumentOutOfRangeException(nameof(userRoleId));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                UserRoleId = userRoleId
            };

            await InsertAclRecordAsync(aclRecord);
        }

        /// <summary>
        /// Get a value indicating whether any ACL records exist for entity type are related to user roles
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="userRoleIds">User's role identifiers</param>
        /// <returns>True if exist; otherwise false</returns>
        public virtual async Task<bool> IsEntityAclMappingExistAsync<TEntity>(int[] userRoleIds) where TEntity : BaseEntity, IAclSupported
        {
            if (!userRoleIds.Any())
                return false;

            var entityName = typeof(TEntity).Name;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgSecurityDefaults.EntityAclRecordExistsCacheKey, entityName, userRoleIds);

            var query = from acl in _aclRecordRepository.Table
                        where acl.EntityName == entityName &&
                              userRoleIds.Contains(acl.UserRoleId)
                        select acl;

            return await _staticCacheManager.GetAsync(key, query.Any);
        }

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>User role identifiers</returns>
        public virtual async Task<int[]> GetUserRoleIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgSecurityDefaults.AclRecordCacheKey, entityId, entityName);

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName
                        select ur.UserRoleId;

            return await _staticCacheManager.GetAsync(key, () => query.ToArray());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
        {
            return await AuthorizeAsync(entity, await _workContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, User user) where TEntity : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (user == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in await _userService.GetUserRolesAsync(user))
                foreach (var role2Id in await GetUserRoleIdsWithAccessAsync(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        #endregion
    }
}