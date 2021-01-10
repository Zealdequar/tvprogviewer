using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Caching.CachingDefaults;
using TVProgViewer.Services.Caching.Extensions;
using TVProgViewer.Services.Events;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User attribute service
    /// </summary>
    public partial class UserAttributeService : IUserAttributeService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<UserAttribute> _userAttributeRepository;
        private readonly IRepository<UserAttributeValue> _userAttributeValueRepository;

        #endregion

        #region Ctor

        public UserAttributeService(IEventPublisher eventPublisher,
            IRepository<UserAttribute> userAttributeRepository,
            IRepository<UserAttributeValue> UserAttributeValueRepository)
        {
            _eventPublisher = eventPublisher;
            _userAttributeRepository = userAttributeRepository;
            _userAttributeValueRepository = UserAttributeValueRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a User attribute
        /// </summary>
        /// <param name="UserAttribute">User attribute</param>
        public virtual void DeleteUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            _userAttributeRepository.Delete(userAttribute);

            //event notification
            _eventPublisher.EntityDeleted(userAttribute);
        }

        /// <summary>
        /// Gets all User attributes
        /// </summary>
        /// <returns>User attributes</returns>
        public virtual IList<UserAttribute> GetAllUserAttributes()
        {
            var query = from ca in _userAttributeRepository.Table
                orderby ca.DisplayOrder, ca.Id
                select ca;

            return query.ToCachedList(TvProgUserServiceCachingDefaults.UserAttributesAllCacheKey);
        }

        /// <summary>
        /// Gets a User attribute 
        /// </summary>
        /// <param name="UserAttributeId">User attribute identifier</param>
        /// <returns>User attribute</returns>
        public virtual UserAttribute GetUserAttributeById(int userAttributeId)
        {
            if (userAttributeId == 0)
                return null;

            return _userAttributeRepository.ToCachedGetById(userAttributeId);
        }

        /// <summary>
        /// Inserts a User attribute
        /// </summary>
        /// <param name="UserAttribute">User attribute</param>
        public virtual void InsertUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(UserAttribute));

            _userAttributeRepository.Insert(userAttribute);
            
            //event notification
            _eventPublisher.EntityInserted(userAttribute);
        }

        /// <summary>
        /// Updates the User attribute
        /// </summary>
        /// <param name="UserAttribute">User attribute</param>
        public virtual void UpdateUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            _userAttributeRepository.Update(userAttribute);

            //event notification
            _eventPublisher.EntityUpdated(userAttribute);
        }

        /// <summary>
        /// Deletes a User attribute value
        /// </summary>
        /// <param name="UserAttributeValue">User attribute value</param>
        public virtual void DeleteUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Delete(userAttributeValue);

            //event notification
            _eventPublisher.EntityDeleted(userAttributeValue);
        }

        /// <summary>
        /// Gets User attribute values by User attribute identifier
        /// </summary>
        /// <param name="UserAttributeId">The User attribute identifier</param>
        /// <returns>User attribute values</returns>
        public virtual IList<UserAttributeValue> GetUserAttributeValues(int userAttributeId)
        {
            var key = TvProgUserServiceCachingDefaults.UserAttributeValuesAllCacheKey.FillCacheKey(userAttributeId);

            var query = from cav in _userAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.UserAttributeId == userAttributeId
                select cav;
            var userAttributeValues = query.ToCachedList(key);

            return userAttributeValues;
        }

        /// <summary>
        /// Gets a User attribute value
        /// </summary>
        /// <param name="UserAttributeValueId">User attribute value identifier</param>
        /// <returns>User attribute value</returns>
        public virtual UserAttributeValue GetUserAttributeValueById(int userAttributeValueId)
        {
            if (userAttributeValueId == 0)
                return null;

            return _userAttributeValueRepository.ToCachedGetById(userAttributeValueId);
        }

        /// <summary>
        /// Inserts a User attribute value
        /// </summary>
        /// <param name="UserAttributeValue">User attribute value</param>
        public virtual void InsertUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Insert(userAttributeValue);

            //event notification
            _eventPublisher.EntityInserted(userAttributeValue);
        }

        /// <summary>
        /// Updates the User attribute value
        /// </summary>
        /// <param name="UserAttributeValue">User attribute value</param>
        public virtual void UpdateUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Update(userAttributeValue);

            //event notification
            _eventPublisher.EntityUpdated(userAttributeValue);
        }

        #endregion
    }
}