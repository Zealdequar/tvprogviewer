using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute service
    /// </summary>
    public partial class TvChannelAttributeService : ITvChannelAttributeService
    {
        #region Fields

        private readonly IRepository<PredefinedTvChannelAttributeValue> _predefinedTvChannelAttributeValueRepository;
        private readonly IRepository<TvChannel> _tvchannelRepository;
        private readonly IRepository<TvChannelAttribute> _tvchannelAttributeRepository;
        private readonly IRepository<TvChannelAttributeCombination> _tvchannelAttributeCombinationRepository;
        private readonly IRepository<TvChannelAttributeMapping> _tvchannelAttributeMappingRepository;
        private readonly IRepository<TvChannelAttributeValue> _tvchannelAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public TvChannelAttributeService(IRepository<PredefinedTvChannelAttributeValue> predefinedTvChannelAttributeValueRepository,
            IRepository<TvChannel> tvchannelRepository,
            IRepository<TvChannelAttribute> tvchannelAttributeRepository,
            IRepository<TvChannelAttributeCombination> tvchannelAttributeCombinationRepository,
            IRepository<TvChannelAttributeMapping> tvchannelAttributeMappingRepository,
            IRepository<TvChannelAttributeValue> tvchannelAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _predefinedTvChannelAttributeValueRepository = predefinedTvChannelAttributeValueRepository;
            _tvchannelRepository = tvchannelRepository;
            _tvchannelAttributeRepository = tvchannelAttributeRepository;
            _tvchannelAttributeCombinationRepository = tvchannelAttributeCombinationRepository;
            _tvchannelAttributeMappingRepository = tvchannelAttributeMappingRepository;
            _tvchannelAttributeValueRepository = tvchannelAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region TvChannel attributes

        /// <summary>
        /// Deletes a tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute)
        {
            await _tvchannelAttributeRepository.DeleteAsync(tvchannelAttribute);
        }

        /// <summary>
        /// Deletes tvchannel attributes
        /// </summary>
        /// <param name="tvchannelAttributes">TvChannel attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributesAsync(IList<TvChannelAttribute> tvchannelAttributes)
        {
            if (tvchannelAttributes == null)
                throw new ArgumentNullException(nameof(tvchannelAttributes));

            foreach (var tvchannelAttribute in tvchannelAttributes) 
                await DeleteTvChannelAttributeAsync(tvchannelAttribute);
        }

        /// <summary>
        /// Gets all tvchannel attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attributes
        /// </returns>
        public virtual async Task<IPagedList<TvChannelAttribute>> GetAllTvChannelAttributesAsync(int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var tvchannelAttributes = await _tvchannelAttributeRepository.GetAllPagedAsync(query =>
            {
                return from pa in query
                    orderby pa.Name
                    select pa;
            }, pageIndex, pageSize);

            return tvchannelAttributes;
        }

        /// <summary>
        /// Gets a tvchannel attribute 
        /// </summary>
        /// <param name="tvchannelAttributeId">TvChannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute 
        /// </returns>
        public virtual async Task<TvChannelAttribute> GetTvChannelAttributeByIdAsync(int tvchannelAttributeId)
        {
            return await _tvchannelAttributeRepository.GetByIdAsync(tvchannelAttributeId, cache => default);
        }

        /// <summary>
        /// Gets tvchannel attributes 
        /// </summary>
        /// <param name="tvchannelAttributeIds">TvChannel attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attributes 
        /// </returns>
        public virtual async Task<IList<TvChannelAttribute>> GetTvChannelAttributeByIdsAsync(int[] tvchannelAttributeIds)
        {
            return await _tvchannelAttributeRepository.GetByIdsAsync(tvchannelAttributeIds);
        }

        /// <summary>
        /// Inserts a tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute)
        {
            await _tvchannelAttributeRepository.InsertAsync(tvchannelAttribute);
        }

        /// <summary>
        /// Updates the tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute)
        {
            await _tvchannelAttributeRepository.UpdateAsync(tvchannelAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of IDs not existing attributes
        /// </returns>
        public virtual async Task<int[]> GetNotExistingAttributesAsync(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException(nameof(attributeId));

            var query = _tvchannelAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToListAsync();
            
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region TvChannel attributes mappings

        /// <summary>
        /// Deletes a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            await _tvchannelAttributeMappingRepository.DeleteAsync(tvchannelAttributeMapping);
        }

        /// <summary>
        /// Gets tvchannel attribute mappings by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeMapping>> GetTvChannelAttributeMappingsByTvChannelIdAsync(int tvchannelId)
        {
            var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeMappingsByTvChannelCacheKey, tvchannelId);

            var query = from pam in _tvchannelAttributeMappingRepository.Table
                orderby pam.DisplayOrder, pam.Id
                where pam.TvChannelId == tvchannelId
                select pam;

            var attributes = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<TvChannelAttributeMapping>();

            return attributes;
        }

        /// <summary>
        /// Gets a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping
        /// </returns>
        public virtual async Task<TvChannelAttributeMapping> GetTvChannelAttributeMappingByIdAsync(int tvchannelAttributeMappingId)
        {
            return await _tvchannelAttributeMappingRepository.GetByIdAsync(tvchannelAttributeMappingId, cache => default);
        }

        /// <summary>
        /// Inserts a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">The tvchannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            await _tvchannelAttributeMappingRepository.InsertAsync(tvchannelAttributeMapping);
        }

        /// <summary>
        /// Updates the tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">The tvchannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            await _tvchannelAttributeMappingRepository.UpdateAsync(tvchannelAttributeMapping);
        }

        #endregion

        #region TvChannel attribute values

        /// <summary>
        /// Deletes a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">TvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue)
        {
            await _tvchannelAttributeValueRepository.DeleteAsync(tvchannelAttributeValue);
        }

        /// <summary>
        /// Gets tvchannel attribute values by tvchannel attribute mapping identifier
        /// </summary>
        /// <param name="tvchannelAttributeMappingId">The tvchannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeValue>> GetTvChannelAttributeValuesAsync(int tvchannelAttributeMappingId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeValuesByAttributeCacheKey, tvchannelAttributeMappingId);

            var query = from pav in _tvchannelAttributeValueRepository.Table
                orderby pav.DisplayOrder, pav.Id
                where pav.TvChannelAttributeMappingId == tvchannelAttributeMappingId
                select pav;
            var tvchannelAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return tvchannelAttributeValues;
        }

        /// <summary>
        /// Gets a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValueId">TvChannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute value
        /// </returns>
        public virtual async Task<TvChannelAttributeValue> GetTvChannelAttributeValueByIdAsync(int tvchannelAttributeValueId)
        {
            return await _tvchannelAttributeValueRepository.GetByIdAsync(tvchannelAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">The tvchannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue)
        {
            await _tvchannelAttributeValueRepository.InsertAsync(tvchannelAttributeValue);
        }

        /// <summary>
        /// Updates the tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">The tvchannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue)
        {
            await _tvchannelAttributeValueRepository.UpdateAsync(tvchannelAttributeValue);
        }

        #endregion

        #region Predefined tvchannel attribute values

        /// <summary>
        /// Deletes a predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">Predefined tvchannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeletePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.DeleteAsync(ppav);
        }

        /// <summary>
        /// Gets predefined tvchannel attribute values by tvchannel attribute identifier
        /// </summary>
        /// <param name="tvchannelAttributeId">The tvchannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<PredefinedTvChannelAttributeValue>> GetPredefinedTvChannelAttributeValuesAsync(int tvchannelAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.PredefinedTvChannelAttributeValuesByAttributeCacheKey, tvchannelAttributeId);

            var query = from ppav in _predefinedTvChannelAttributeValueRepository.Table
                        orderby ppav.DisplayOrder, ppav.Id
                        where ppav.TvChannelAttributeId == tvchannelAttributeId
                        select ppav;

            var values = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return values;
        }

        /// <summary>
        /// Gets a predefined tvchannel attribute value
        /// </summary>
        /// <param name="id">Predefined tvchannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvchannel attribute value
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValue> GetPredefinedTvChannelAttributeValueByIdAsync(int id)
        {
            return await _predefinedTvChannelAttributeValueRepository.GetByIdAsync(id, cache => default);
        }

        /// <summary>
        /// Inserts a predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvchannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertPredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.InsertAsync(ppav);
        }

        /// <summary>
        /// Updates the predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvchannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdatePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.UpdateAsync(ppav);
        }

        #endregion

        #region TvChannel attribute combinations

        /// <summary>
        /// Deletes a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvchannelAttributeCombinationRepository.DeleteAsync(combination);
        }

        /// <summary>
        /// Gets all tvchannel attribute combinations
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute combinations
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeCombination>> GetAllTvChannelAttributeCombinationsAsync(int tvchannelId)
        {
            if (tvchannelId == 0)
                return new List<TvChannelAttributeCombination>();

            var combinations = await _tvchannelAttributeCombinationRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.Id
                    where c.TvChannelId == tvchannelId
                    select c;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeCombinationsByTvChannelCacheKey, tvchannelId));

            return combinations;
        }

        /// <summary>
        /// Gets a tvchannel attribute combination
        /// </summary>
        /// <param name="tvchannelAttributeCombinationId">TvChannel attribute combination identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute combination
        /// </returns>
        public virtual async Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationByIdAsync(int tvchannelAttributeCombinationId)
        {
            return await _tvchannelAttributeCombinationRepository.GetByIdAsync(tvchannelAttributeCombinationId, cache => default);
        }

        /// <summary>
        /// Gets a tvchannel attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute combination
        /// </returns>
        public virtual async Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _tvchannelAttributeCombinationRepository.Table
                        join p in _tvchannelRepository.Table on pac.TvChannelId equals p.Id
                        orderby pac.Id
                        where !p.Deleted && pac.Sku == sku
                        select pac;
            var combination = await query.FirstOrDefaultAsync();

            return combination;
        }

        /// <summary>
        /// Inserts a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvchannelAttributeCombinationRepository.InsertAsync(combination);
        }

        /// <summary>
        /// Updates a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvchannelAttributeCombinationRepository.UpdateAsync(combination);
        }

        #endregion

        #endregion
    }
}