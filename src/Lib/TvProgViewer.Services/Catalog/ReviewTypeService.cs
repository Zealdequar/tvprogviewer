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

        private readonly IRepository<TvChannelReviewReviewTypeMapping> _tvchannelReviewReviewTypeMappingRepository;
        private readonly IRepository<ReviewType> _reviewTypeRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ReviewTypeService(IRepository<TvChannelReviewReviewTypeMapping> tvchannelReviewReviewTypeMappingRepository,
            IRepository<ReviewType> reviewTypeRepository,
            IStaticCacheManager staticCacheManager)
        {
            _tvchannelReviewReviewTypeMappingRepository = tvchannelReviewReviewTypeMappingRepository;
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
        /// A task that represents the asynchronous operation
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
        /// A task that represents the asynchronous operation
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.InsertAsync(reviewType);
        }

        /// <summary>
        /// Updates a review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.UpdateAsync(reviewType);
        }

        /// <summary>
        /// Delete review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteReviewTypeAsync(ReviewType reviewType)
        {
            await _reviewTypeRepository.DeleteAsync(reviewType);
        }

        #endregion

        #region TvChannel review type mapping

        /// <summary>
        /// Gets tvchannel review and review type mappings by tvchannel review identifier
        /// </summary>
        /// <param name="tvchannelReviewId">The tvchannel review identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel review and review type mapping collection
        /// </returns>
        public async Task<IList<TvChannelReviewReviewTypeMapping>> GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(
            int tvchannelReviewId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelReviewTypeMappingByReviewTypeCacheKey, tvchannelReviewId);

            var query = from pam in _tvchannelReviewReviewTypeMappingRepository.Table
                orderby pam.Id
                where pam.TvChannelReviewId == tvchannelReviewId
                select pam;

            var tvchannelReviewReviewTypeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return tvchannelReviewReviewTypeMappings;
        }

        /// <summary>
        /// Inserts a tvchannel review and review type mapping
        /// </summary>
        /// <param name="tvchannelReviewReviewType">TvChannel review and review type mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTvChannelReviewReviewTypeMappingsAsync(TvChannelReviewReviewTypeMapping tvchannelReviewReviewType)
        {
            await _tvchannelReviewReviewTypeMappingRepository.InsertAsync(tvchannelReviewReviewType);
        }

        #endregion

        #endregion
    }
}