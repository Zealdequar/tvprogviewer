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
        private readonly IRepository<TvChannel> _tvchannelRepository;
        private readonly IRepository<TvChannelTvChannelTagMapping> _tvchannelTvChannelTagMappingRepository;
        private readonly IRepository<TvChannelTag> _tvchannelTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TvChannelTagService(
            IAclService aclService,
            IUserService userService,
            IRepository<TvChannel> tvchannelRepository,
            IRepository<TvChannelTvChannelTagMapping> tvchannelTvChannelTagMappingRepository,
            IRepository<TvChannelTag> tvchannelTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userService = userService;
            _tvchannelRepository = tvchannelRepository;
            _tvchannelTvChannelTagMappingRepository = tvchannelTvChannelTagMappingRepository;
            _tvchannelTagRepository = tvchannelTagRepository;
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
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteTvChannelTvChannelTagMappingAsync(int tvchannelId, int tvchannelTagId)
        {
            var mappingRecord = await _tvchannelTvChannelTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.TvChannelId == tvchannelId && pptm.TvChannelTagId == tvchannelTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _tvchannelTvChannelTagMappingRepository.DeleteAsync(mappingRecord);
        }

        /// <summary>
        /// Indicates whether a tvchannel tag exists
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> TvChannelTagExistsAsync(TvChannel tvchannel, int tvchannelTagId)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            return await _tvchannelTvChannelTagMappingRepository.Table
                .AnyAsync(pptm => pptm.TvChannelId == tvchannel.Id && pptm.TvChannelTagId == tvchannelTagId);
        }

        /// <summary>
        /// Gets tvchannel tag by name
        /// </summary>
        /// <param name="name">TvChannel tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag
        /// </returns>
        protected virtual async Task<TvChannelTag> GetTvChannelTagByNameAsync(string name)
        {
            var query = from pt in _tvchannelTagRepository.Table
                where pt.Name == name
                select pt;

            var tvchannelTag = await query.FirstOrDefaultAsync();
            return tvchannelTag;
        }

        /// <summary>
        /// Inserts a tvchannel tag
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertTvChannelTagAsync(TvChannelTag tvchannelTag)
        {
            await _tvchannelTagRepository.InsertAsync(tvchannelTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a tvchannel tag
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTvChannelTagAsync(TvChannelTag tvchannelTag)
        {
            await _tvchannelTagRepository.DeleteAsync(tvchannelTag);
        }

        /// <summary>
        /// Delete tvchannel tags
        /// </summary>
        /// <param name="tvchannelTags">TvChannel tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTvChannelTagsAsync(IList<TvChannelTag> tvchannelTags)
        {
            if (tvchannelTags == null)
                throw new ArgumentNullException(nameof(tvchannelTags));

            foreach (var tvchannelTag in tvchannelTags)
                await DeleteTvChannelTagAsync(tvchannelTag);
        }

        /// <summary>
        /// Gets all tvchannel tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetAllTvChannelTagsAsync(string tagName = null)
        {
            var allTvChannelTags = await _tvchannelTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allTvChannelTags = allTvChannelTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allTvChannelTags;
        }

        /// <summary>
        /// Gets all tvchannel tags by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetAllTvChannelTagsByTvChannelIdAsync(int tvchannelId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelTagsByTvChannelCacheKey, tvchannelId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _tvchannelTvChannelTagMappingRepository.Table
                                 join pt in _tvchannelTagRepository.Table on ptm.TvChannelTagId equals pt.Id
                                 where ptm.TvChannelId == tvchannelId
                                 orderby pt.Id
                                 select pt;

                return await tagMapping.ToListAsync();
            });
        }

        /// <summary>
        /// Gets tvchannel tag
        /// </summary>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag
        /// </returns>
        public virtual async Task<TvChannelTag> GetTvChannelTagByIdAsync(int tvchannelTagId)
        {
            return await _tvchannelTagRepository.GetByIdAsync(tvchannelTagId, cache => default);
        }

        /// <summary>
        /// Gets tvchannel tags
        /// </summary>
        /// <param name="tvchannelTagIds">TvChannel tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        public virtual async Task<IList<TvChannelTag>> GetTvChannelTagsByIdsAsync(int[] tvchannelTagIds)
        {
            return await _tvchannelTagRepository.GetByIdsAsync(tvchannelTagIds);
        }
        
        /// <summary>
        /// Inserts a tvchannel-tvchannel tag mapping
        /// </summary>
        /// <param name="tagMapping">TvChannel-tvchannel tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTvChannelTvChannelTagMappingAsync(TvChannelTvChannelTagMapping tagMapping)
        {
            await _tvchannelTvChannelTagMappingRepository.InsertAsync(tagMapping);
        }
        
        /// <summary>
        /// Updates the tvchannel tag
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTvChannelTagAsync(TvChannelTag tvchannelTag)
        {
            if (tvchannelTag == null)
                throw new ArgumentNullException(nameof(tvchannelTag));

            await _tvchannelTagRepository.UpdateAsync(tvchannelTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(tvchannelTag, string.Empty, tvchannelTag.Name, true);
            await _urlRecordService.SaveSlugAsync(tvchannelTag, seName, 0);
        }

        /// <summary>
        /// Get tvchannels quantity linked to a passed tag identifier
        /// </summary>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of tvchannels
        /// </returns>
        public virtual async Task<int> GetTvChannelCountByTvChannelTagIdAsync(int tvchannelTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetTvChannelCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(tvchannelTagId))
                return dictionary[tvchannelTagId];

            return 0;
        }

        /// <summary>
        /// Get tvchannel count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary of "tvchannel tag ID : tvchannel count"
        /// </returns>
        public virtual async Task<Dictionary<int, int>> GetTvChannelCountAsync(int storeId, bool showHidden = false)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelTagCountCacheKey, storeId, userRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _tvchannelTvChannelTagMappingRepository.Table;

                if (!showHidden)
                {
                    var tvchannelsQuery = _tvchannelRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    tvchannelsQuery = await _storeMappingService.ApplyStoreMapping(tvchannelsQuery, storeId);

                    //apply ACL constraints
                    tvchannelsQuery = await _aclService.ApplyAcl(tvchannelsQuery, userRoleIds);

                    query = query.Where(pc => tvchannelsQuery.Any(p => !p.Deleted && pc.TvChannelId == p.Id));
                }

                var pTagCount = from pt in _tvchannelTagRepository.Table
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
        /// Update tvchannel tags
        /// </summary>
        /// <param name="tvchannel">TvChannel for update</param>
        /// <param name="tvchannelTags">TvChannel tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTvChannelTagsAsync(TvChannel tvchannel, string[] tvchannelTags)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //tvchannel tags
            var existingTvChannelTags = await GetAllTvChannelTagsByTvChannelIdAsync(tvchannel.Id);
            var tvchannelTagsToRemove = new List<TvChannelTag>();
            foreach (var existingTvChannelTag in existingTvChannelTags)
            {
                var found = false;
                foreach (var newTvChannelTag in tvchannelTags)
                {
                    if (!existingTvChannelTag.Name.Equals(newTvChannelTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    tvchannelTagsToRemove.Add(existingTvChannelTag);
            }

            foreach (var tvchannelTag in tvchannelTagsToRemove)
                await DeleteTvChannelTvChannelTagMappingAsync(tvchannel.Id, tvchannelTag.Id);

            foreach (var tvchannelTagName in tvchannelTags)
            {
                TvChannelTag tvchannelTag;
                var tvchannelTag2 = await GetTvChannelTagByNameAsync(tvchannelTagName);
                if (tvchannelTag2 == null)
                {
                    //add new tvchannel tag
                    tvchannelTag = new TvChannelTag
                    {
                        Name = tvchannelTagName
                    };
                    await InsertTvChannelTagAsync(tvchannelTag);
                }
                else
                    tvchannelTag = tvchannelTag2;

                if (!await TvChannelTagExistsAsync(tvchannel, tvchannelTag.Id))
                    await InsertTvChannelTvChannelTagMappingAsync(new TvChannelTvChannelTagMapping { TvChannelTagId = tvchannelTag.Id, TvChannelId = tvchannel.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(tvchannelTag, string.Empty, tvchannelTag.Name, true);
                await _urlRecordService.SaveSlugAsync(tvchannelTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(TvProgEntityCacheDefaults<TvChannelTag>.Prefix);
        }

        #endregion
    }
}