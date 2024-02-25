using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserService _userService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicService(
            IAclService aclService,
            IUserService userService,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userService = userService;
            _topicRepository = topicRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTopicAsync(Topic topic)
        {
            await _topicRepository.DeleteAsync(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic
        /// </returns>
        public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await _topicRepository.GetByIdAsync(topicId, cache => default);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the topic
        /// </returns>
        public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, int storeId = 0)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgTopicDefaults.TopicBySystemNameCacheKey, systemName, storeId, userRoleIds);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = _topicRepository.Table
                    .Where(t => t.Published);

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, userRoleIds);

                return query.Where(t => t.SystemName == systemName)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the topics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            return await _topicRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                {
                    query = query.Where(t => t.Published);

                    //apply store mapping constraints
                    query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                    //apply ACL constraints
                    if (!ignoreAcl)
                        query = await _aclService.ApplyAcl(query, userRoleIds);
                }

                if (onlyIncludedInTopMenu)
                    query = query.Where(t => t.IncludeInTopMenu);

                return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
            }, cache =>
            {
                return ignoreAcl
                    ? cache.PrepareKeyForDefaultCache(TvProgTopicDefaults.TopicsAllCacheKey, storeId, showHidden, onlyIncludedInTopMenu)
                    : cache.PrepareKeyForDefaultCache(TvProgTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, onlyIncludedInTopMenu, userRoleIds);
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search into body or title</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId, string keywords,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = await GetAllTopicsAsync(storeId,
                ignoreAcl: ignoreAcl,
                showHidden: showHidden,
                onlyIncludedInTopMenu: onlyIncludedInTopMenu);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                return topics
                    .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                        (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
                    .ToList();
            }

            return topics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTopicAsync(Topic topic)
        {
            await _topicRepository.InsertAsync(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTopicAsync(Topic topic)
        {
            await _topicRepository.UpdateAsync(topic);
        }

        #endregion
    }
}