using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Review type service implementation
    /// </summary>
    public partial class ReviewTypeService : IReviewTypeService
    {
        #region Fields

        private readonly IRepository<TvChannelReviewReviewTypeMapping> _tvChannelReviewReviewTypeMappingRepository;
        private readonly IRepository<ReviewType> _reviewTypeRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ReviewTypeService(IRepository<TvChannelReviewReviewTypeMapping> tvChannelReviewReviewTypeMappingRepository,
            IRepository<ReviewType> reviewTypeRepository,
            IStaticCacheManager staticCacheManager)
        {
            _tvChannelReviewReviewTypeMappingRepository = tvChannelReviewReviewTypeMappingRepository;
            _reviewTypeRepository = reviewTypeRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Review type

        /// <summary>
        /// Gets all review types
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the review types
        /// </returns>
        public virtual async Task<IList<ReviewType>> GetAllReviewTypesAsync()
        {
            return await _reviewTypeRepository.GetAllAsync(
                query => query.OrderBy(reviewType => reviewType.DisplayOrder).ThenBy(reviewType => reviewType.Id),
                cache => default);
        }

        /// <summary>
        /// Gets a review type 
        /// </summary>
        /// <param name="reviewTypeId">Review type identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the review type
        /// </returns>
        public virtual async Task<ReviewType> GetReviewTypeByIdAsync(int reviewTypeId)
        {
            return await _reviewTypeRepository.GetByIdAsync(reviewTypeId, cache => default);
        }

        /// <summary>
        /// Inserts a review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.InsertAsync(reviewType);
        }

        /// <summary>
        /// Updates a review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.UpdateAsync(reviewType);
        }

        /// <summary>
        /// Delete review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.DeleteAsync(reviewType);
        }

        #endregion

        #region TvChannel review type mapping

        /// <summary>
        /// Gets tvChannel review and review type mappings by tvChannel review identifier
        /// </summary>
        /// <param name="tvChannelReviewId">The tvChannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review and review type mapping collection
        /// </returns>
        public async Task<IList<TvChannelReviewReviewTypeMapping>> GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(
            int tvChannelReviewId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelReviewTypeMappingByReviewTypeCacheKey, tvChannelReviewId);

            var query = from pam in _tvChannelReviewReviewTypeMappingRepository.Table
                orderby pam.Id
                where pam.TvChannelReviewId == tvChannelReviewId
                select pam;

            var tvChannelReviewReviewTypeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return tvChannelReviewReviewTypeMappings;
        }

        /// <summary>
        /// Inserts a tvChannel review and review type mapping
        /// </summary>
        /// <param name="tvChannelReviewReviewType">TvChannel review and review type mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelReviewReviewTypeMappingsAsync(TvChannelReviewReviewTypeMapping tvChannelReviewReviewType)
        {
            await _tvChannelReviewReviewTypeMappingRepository.InsertAsync(tvChannelReviewReviewType);
        }

        #endregion

        #endregion
    }
}