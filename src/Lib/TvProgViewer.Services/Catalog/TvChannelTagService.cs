using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel tag service
    /// </summary>
    public partial class TvChannelTagService : ITvChannelTagService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserService _userService;
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<TvChannelTvChannelTagMapping> _tvChannelTvChannelTagMappingRepository;
        private readonly IRepository<TvChannelTag> _tvChannelTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TvChannelTagService(
            IAclService aclService,
            IUserService userService,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<TvChannelTvChannelTagMapping> tvChannelTvChannelTagMappingRepository,
            IRepository<TvChannelTag> tvChannelTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userService = userService;
            _tvChannelRepository = tvChannelRepository;
            _tvChannelTvChannelTagMappingRepository = tvChannelTvChannelTagMappingRepository;
            _tvChannelTagRepository = tvChannelTagRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a tvchannel-tvchannel tag mapping
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task DeleteTvChannelTvChannelTagMappingAsync(int tvChannelId, int tvChannelTagId)
        {
            var mappingRecord = await _tvChannelTvChannelTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.TvChannelId == tvChannelId && pptm.TvChannelTagId == tvChannelTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _tvChannelTvChannelTagMappingRepository.DeleteAsync(mappingRecord);
        }

        /// <summary>
        /// Indicates whether a tvChannel tag exists
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> TvChannelTagExistsAsync(TvChannel tvChannel, int tvChannelTagId)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            return await _tvChannelTvChannelTagMappingRepository.Table
                .AnyAsync(pptm => pptm.TvChannelId == tvChannel.Id && pptm.TvChannelTagId == tvChannelTagId);
        }

        /// <summary>
        /// Gets tvChannel tag by name
        /// </summary>
        /// <param name="name">TvChannel tag name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag
        /// </returns>
        protected virtual async Task<TvChannelTag> GetTvChannelTagByNameAsync(string name)
        {
            var query = from pt in _tvChannelTagRepository.Table
                where pt.Name == name
                select pt;

            var tvChannelTag = await query.FirstOrDefaultAsync();
            return tvChannelTag;
        }

        /// <summary>
        /// Inserts a tvChannel tag
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertTvChannelTagAsync(TvChannelTag tvChannelTag)
        {
            await _tvChannelTagRepository.InsertAsync(tvChannelTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a tvChannel tag
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelTagAsync(TvChannelTag tvChannelTag)
        {
            await _tvChannelTagRepository.DeleteAsync(tvChannelTag);
        }

        /// <summary>
        /// Delete tvChannel tags
        /// </summary>
        /// <param name="tvChannelTags">TvChannel tags</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelTagsAsync(IList<TvChannelTag> tvChannelTags)
        {
            if (tvChannelTags == null)
                throw new ArgumentNullException(nameof(tvChannelTags));

            foreach (var tvChannelTag in tvChannelTags)
                await DeleteTvChannelTagAsync(tvChannelTag);
        }

        /// <summary>
        /// Gets all tvChannel tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetAllTvChannelTagsAsync(string tagName = null)
        {
            var allTvChannelTags = await _tvChannelTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allTvChannelTags = allTvChannelTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allTvChannelTags;
        }

        /// <summary>
        /// Gets all tvChannel tags by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetAllTvChannelTagsByTvChannelIdAsync(int tvChannelId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelTagsByTvChannelCacheKey, tvChannelId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _tvChannelTvChannelTagMappingRepository.Table
                                 join pt in _tvChannelTagRepository.Table on ptm.TvChannelTagId equals pt.Id
                                 where ptm.TvChannelId == tvChannelId
                                 orderby pt.Id
                                 select pt;

                return await tagMapping.ToListAsync();
            });
        }

        /// <summary>
        /// Gets tvChannel tag
        /// </summary>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag
        /// </returns>
        public virtual async Task<TvChannelTag> GetTvChannelTagByIdAsync(int tvChannelTagId)
        {
            return await _tvChannelTagRepository.GetByIdAsync(tvChannelTagId, cache => default);
        }

        /// <summary>
        /// Gets tvChannel tags
        /// </summary>
        /// <param name="tvChannelTagIds">TvChannel tags identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetTvChannelTagsByIdsAsync(int[] tvChannelTagIds)
        {
            return await _tvChannelTagRepository.GetByIdsAsync(tvChannelTagIds);
        }
        
        /// <summary>
        /// Inserts a tvchannel-tvchannel tag mapping
        /// </summary>
        /// <param name="tagMapping">TvChannel-tvchannel tag mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelTvChannelTagMappingAsync(TvChannelTvChannelTagMapping tagMapping)
        {
            await _tvChannelTvChannelTagMappingRepository.InsertAsync(tagMapping);
        }
        
        /// <summary>
        /// Updates the tvChannel tag
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelTagAsync(TvChannelTag tvChannelTag)
        {
            if (tvChannelTag == null)
                throw new ArgumentNullException(nameof(tvChannelTag));

            await _tvChannelTagRepository.UpdateAsync(tvChannelTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(tvChannelTag, string.Empty, tvChannelTag.Name, true);
            await _urlRecordService.SaveSlugAsync(tvChannelTag, seName, 0);
        }

        /// <summary>
        /// Get tvChannels quantity linked to a passed tag identifier
        /// </summary>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        public virtual async Task<int> GetTvChannelCountByTvChannelTagIdAsync(int tvChannelTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetTvChannelCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(tvChannelTagId))
                return dictionary[tvChannelTagId];

            return 0;
        }

        /// <summary>
        /// Get tvChannel count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the dictionary of "tvChannel tag ID : tvChannel count"
        /// </returns>
        public virtual async Task<Dictionary<int, int>> GetTvChannelCountAsync(int storeId, bool showHidden = false)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelTagCountCacheKey, storeId, userRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _tvChannelTvChannelTagMappingRepository.Table;

                if (!showHidden)
                {
                    var tvChannelsQuery = _tvChannelRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    tvChannelsQuery = await _storeMappingService.ApplyStoreMapping(tvChannelsQuery, storeId);

                    //apply ACL constraints
                    tvChannelsQuery = await _aclService.ApplyAcl(tvChannelsQuery, userRoleIds);

                    query = query.Where(pc => tvChannelsQuery.Any(p => !p.Deleted && pc.TvChannelId == p.Id));
                }

                var pTagCount = from pt in _tvChannelTagRepository.Table
                                join ptm in query on pt.Id equals ptm.TvChannelTagId
                                group ptm by ptm.TvChannelTagId into ptmGrouped
                                select new
                                {
                                    TvChannelTagId = ptmGrouped.Key,
                                    TvChannelCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.TvChannelTagId, item => item.TvChannelCount);
            });
        }
        
        /// <summary>
        /// Update tvChannel tags
        /// </summary>
        /// <param name="tvChannel">TvChannel for update</param>
        /// <param name="tvChannelTags">TvChannel tags</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelTagsAsync(TvChannel tvChannel, string[] tvChannelTags)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //tvChannel tags
            var existingTvChannelTags = await GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id);
            var tvChannelTagsToRemove = new List<TvChannelTag>();
            foreach (var existingTvChannelTag in existingTvChannelTags)
            {
                var found = false;
                foreach (var newTvChannelTag in tvChannelTags)
                {
                    if (!existingTvChannelTag.Name.Equals(newTvChannelTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    tvChannelTagsToRemove.Add(existingTvChannelTag);
            }

            foreach (var tvChannelTag in tvChannelTagsToRemove)
                await DeleteTvChannelTvChannelTagMappingAsync(tvChannel.Id, tvChannelTag.Id);

            foreach (var tvChannelTagName in tvChannelTags)
            {
                TvChannelTag tvChannelTag;
                var tvChannelTag2 = await GetTvChannelTagByNameAsync(tvChannelTagName);
                if (tvChannelTag2 == null)
                {
                    //add new tvChannel tag
                    tvChannelTag = new TvChannelTag
                    {
                        Name = tvChannelTagName
                    };
                    await InsertTvChannelTagAsync(tvChannelTag);
                }
                else
                    tvChannelTag = tvChannelTag2;

                if (!await TvChannelTagExistsAsync(tvChannel, tvChannelTag.Id))
                    await InsertTvChannelTvChannelTagMappingAsync(new TvChannelTvChannelTagMapping { TvChannelTagId = tvChannelTag.Id, TvChannelId = tvChannel.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(tvChannelTag, string.Empty, tvChannelTag.Name, true);
                await _urlRecordService.SaveSlugAsync(tvChannelTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(TvProgEntityCacheDefaults<TvChannelTag>.Prefix);
        }

        #endregion
    }
}