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
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<TvChannelAttribute> _tvChannelAttributeRepository;
        private readonly IRepository<TvChannelAttributeCombination> _tvChannelAttributeCombinationRepository;
        private readonly IRepository<TvChannelAttributeMapping> _tvChannelAttributeMappingRepository;
        private readonly IRepository<TvChannelAttributeValue> _tvChannelAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public TvChannelAttributeService(IRepository<PredefinedTvChannelAttributeValue> predefinedTvChannelAttributeValueRepository,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<TvChannelAttribute> tvChannelAttributeRepository,
            IRepository<TvChannelAttributeCombination> tvChannelAttributeCombinationRepository,
            IRepository<TvChannelAttributeMapping> tvChannelAttributeMappingRepository,
            IRepository<TvChannelAttributeValue> tvChannelAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _predefinedTvChannelAttributeValueRepository = predefinedTvChannelAttributeValueRepository;
            _tvChannelRepository = tvChannelRepository;
            _tvChannelAttributeRepository = tvChannelAttributeRepository;
            _tvChannelAttributeCombinationRepository = tvChannelAttributeCombinationRepository;
            _tvChannelAttributeMappingRepository = tvChannelAttributeMappingRepository;
            _tvChannelAttributeValueRepository = tvChannelAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region TvChannel attributes

        /// <summary>
        /// Deletes a tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute)
        {
            await _tvChannelAttributeRepository.DeleteAsync(tvChannelAttribute);
        }

        /// <summary>
        /// Deletes tvChannel attributes
        /// </summary>
        /// <param name="tvChannelAttributes">TvChannel attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributesAsync(IList<TvChannelAttribute> tvChannelAttributes)
        {
            if (tvChannelAttributes == null)
                throw new ArgumentNullException(nameof(tvChannelAttributes));

            foreach (var tvChannelAttribute in tvChannelAttributes) 
                await DeleteTvChannelAttributeAsync(tvChannelAttribute);
        }

        /// <summary>
        /// Gets all tvChannel attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attributes
        /// </returns>
        public virtual async Task<IPagedList<TvChannelAttribute>> GetAllTvChannelAttributesAsync(int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var tvChannelAttributes = await _tvChannelAttributeRepository.GetAllPagedAsync(query =>
            {
                return from pa in query
                    orderby pa.Name
                    select pa;
            }, pageIndex, pageSize);

            return tvChannelAttributes;
        }

        /// <summary>
        /// Gets a tvChannel attribute 
        /// </summary>
        /// <param name="tvChannelAttributeId">TvChannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute 
        /// </returns>
        public virtual async Task<TvChannelAttribute> GetTvChannelAttributeByIdAsync(int tvChannelAttributeId)
        {
            return await _tvChannelAttributeRepository.GetByIdAsync(tvChannelAttributeId, cache => default);
        }

        /// <summary>
        /// Gets tvChannel attributes 
        /// </summary>
        /// <param name="tvChannelAttributeIds">TvChannel attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attributes 
        /// </returns>
        public virtual async Task<IList<TvChannelAttribute>> GetTvChannelAttributeByIdsAsync(int[] tvChannelAttributeIds)
        {
            return await _tvChannelAttributeRepository.GetByIdsAsync(tvChannelAttributeIds);
        }

        /// <summary>
        /// Inserts a tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute)
        {
            await _tvChannelAttributeRepository.InsertAsync(tvChannelAttribute);
        }

        /// <summary>
        /// Updates the tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute)
        {
            await _tvChannelAttributeRepository.UpdateAsync(tvChannelAttribute);
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

            var query = _tvChannelAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToListAsync();
            
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region TvChannel attributes mappings

        /// <summary>
        /// Deletes a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            await _tvChannelAttributeMappingRepository.DeleteAsync(tvChannelAttributeMapping);
        }

        /// <summary>
        /// Gets tvChannel attribute mappings by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeMapping>> GetTvChannelAttributeMappingsByTvChannelIdAsync(int tvChannelId)
        {
            var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeMappingsByTvChannelCacheKey, tvChannelId);

            var query = from pam in _tvChannelAttributeMappingRepository.Table
                orderby pam.DisplayOrder, pam.Id
                where pam.TvChannelId == tvChannelId
                select pam;

            var attributes = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<TvChannelAttributeMapping>();

            return attributes;
        }

        /// <summary>
        /// Gets a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping
        /// </returns>
        public virtual async Task<TvChannelAttributeMapping> GetTvChannelAttributeMappingByIdAsync(int tvChannelAttributeMappingId)
        {
            return await _tvChannelAttributeMappingRepository.GetByIdAsync(tvChannelAttributeMappingId, cache => default);
        }

        /// <summary>
        /// Inserts a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">The tvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            await _tvChannelAttributeMappingRepository.InsertAsync(tvChannelAttributeMapping);
        }

        /// <summary>
        /// Updates the tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">The tvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            await _tvChannelAttributeMappingRepository.UpdateAsync(tvChannelAttributeMapping);
        }

        #endregion

        #region TvChannel attribute values

        /// <summary>
        /// Deletes a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">TvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue)
        {
            await _tvChannelAttributeValueRepository.DeleteAsync(tvChannelAttributeValue);
        }

        /// <summary>
        /// Gets tvChannel attribute values by tvChannel attribute mapping identifier
        /// </summary>
        /// <param name="tvChannelAttributeMappingId">The tvChannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeValue>> GetTvChannelAttributeValuesAsync(int tvChannelAttributeMappingId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeValuesByAttributeCacheKey, tvChannelAttributeMappingId);

            var query = from pav in _tvChannelAttributeValueRepository.Table
                orderby pav.DisplayOrder, pav.Id
                where pav.TvChannelAttributeMappingId == tvChannelAttributeMappingId
                select pav;
            var tvChannelAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return tvChannelAttributeValues;
        }

        /// <summary>
        /// Gets a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValueId">TvChannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value
        /// </returns>
        public virtual async Task<TvChannelAttributeValue> GetTvChannelAttributeValueByIdAsync(int tvChannelAttributeValueId)
        {
            return await _tvChannelAttributeValueRepository.GetByIdAsync(tvChannelAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">The tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue)
        {
            await _tvChannelAttributeValueRepository.InsertAsync(tvChannelAttributeValue);
        }

        /// <summary>
        /// Updates the tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">The tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue)
        {
            await _tvChannelAttributeValueRepository.UpdateAsync(tvChannelAttributeValue);
        }

        #endregion

        #region Predefined tvChannel attribute values

        /// <summary>
        /// Deletes a predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">Predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeletePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.DeleteAsync(ppav);
        }

        /// <summary>
        /// Gets predefined tvChannel attribute values by tvChannel attribute identifier
        /// </summary>
        /// <param name="tvChannelAttributeId">The tvChannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping collection
        /// </returns>
        public virtual async Task<IList<PredefinedTvChannelAttributeValue>> GetPredefinedTvChannelAttributeValuesAsync(int tvChannelAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.PredefinedTvChannelAttributeValuesByAttributeCacheKey, tvChannelAttributeId);

            var query = from ppav in _predefinedTvChannelAttributeValueRepository.Table
                        orderby ppav.DisplayOrder, ppav.Id
                        where ppav.TvChannelAttributeId == tvChannelAttributeId
                        select ppav;

            var values = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return values;
        }

        /// <summary>
        /// Gets a predefined tvChannel attribute value
        /// </summary>
        /// <param name="id">Predefined tvChannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValue> GetPredefinedTvChannelAttributeValueByIdAsync(int id)
        {
            return await _predefinedTvChannelAttributeValueRepository.GetByIdAsync(id, cache => default);
        }

        /// <summary>
        /// Inserts a predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertPredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.InsertAsync(ppav);
        }

        /// <summary>
        /// Updates the predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdatePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav)
        {
            await _predefinedTvChannelAttributeValueRepository.UpdateAsync(ppav);
        }

        #endregion

        #region TvChannel attribute combinations

        /// <summary>
        /// Deletes a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvChannelAttributeCombinationRepository.DeleteAsync(combination);
        }

        /// <summary>
        /// Gets all tvChannel attribute combinations
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combinations
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeCombination>> GetAllTvChannelAttributeCombinationsAsync(int tvChannelId)
        {
            if (tvChannelId == 0)
                return new List<TvChannelAttributeCombination>();

            var combinations = await _tvChannelAttributeCombinationRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.Id
                    where c.TvChannelId == tvChannelId
                    select c;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelAttributeCombinationsByTvChannelCacheKey, tvChannelId));

            return combinations;
        }

        /// <summary>
        /// Gets a tvChannel attribute combination
        /// </summary>
        /// <param name="tvChannelAttributeCombinationId">TvChannel attribute combination identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination
        /// </returns>
        public virtual async Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationByIdAsync(int tvChannelAttributeCombinationId)
        {
            return await _tvChannelAttributeCombinationRepository.GetByIdAsync(tvChannelAttributeCombinationId, cache => default);
        }

        /// <summary>
        /// Gets a tvChannel attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination
        /// </returns>
        public virtual async Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _tvChannelAttributeCombinationRepository.Table
                        join p in _tvChannelRepository.Table on pac.TvChannelId equals p.Id
                        orderby pac.Id
                        where !p.Deleted && pac.Sku == sku
                        select pac;
            var combination = await query.FirstOrDefaultAsync();

            return combination;
        }

        /// <summary>
        /// Inserts a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvChannelAttributeCombinationRepository.InsertAsync(combination);
        }

        /// <summary>
        /// Updates a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination)
        {
            await _tvChannelAttributeCombinationRepository.UpdateAsync(combination);
        }

        #endregion

        #endregion
    }
}