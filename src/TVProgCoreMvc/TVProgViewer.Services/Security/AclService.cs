using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Data;
using TVProgViewer.Services.Caching.CachingDefaults;
using TVProgViewer.Services.Caching.Extensions;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Events;

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
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AclService(CatalogSettings catalogSettings,
            IUserService UserService,
            IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRecordRepository,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _userService = UserService;
            _eventPublisher = eventPublisher;
            _aclRecordRepository = aclRecordRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Delete(aclRecord);

            //event notification
            _eventPublisher.EntityDeleted(aclRecord);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            return _aclRecordRepository.ToCachedGetById(aclRecordId);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = query.ToList();
            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Insert(aclRecord);

            //event notification
            _eventPublisher.EntityInserted(aclRecord);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="UserRoleId">User role id</param>
        /// <param name="entity">Entity</param>
        public virtual void InsertAclRecord<T>(T entity, int UserRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (UserRoleId == 0)
                throw new ArgumentOutOfRangeException(nameof(UserRoleId));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                UserRoleId = UserRoleId
            };

            InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Update(aclRecord);

            //event notification
            _eventPublisher.EntityUpdated(aclRecord);
        }

        /// <summary>
        /// Find User role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>User role identifiers</returns>
        public virtual int[] GetuserRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = TvProgSecurityCachingDefaults.AclRecordByEntityIdNameCacheKey.FillCacheKey(entityId, entityName);

            var query = from ur in _aclRecordRepository.Table
                where ur.EntityId == entityId &&
                      ur.EntityName == entityName
                select ur.UserRoleId;

            return query.ToCachedArray(key);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return Authorize(entity, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="User">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, User User) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (User == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in _userService.GetUserRoles(User))
                foreach (var role2Id in GetuserRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        public int[] GetUserRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}