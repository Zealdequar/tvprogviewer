using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Review type service interface
    /// </summary>
    public partial interface IReviewTypeService
    {
        #region ReviewType

        /// <summary>
        /// Delete the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteReviewTypeAsync(ReviewType reviewType);

        /// <summary>
        /// Get all review types
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the review types
        /// </returns>
        Task<IList<ReviewType>> GetAllReviewTypesAsync();

        /// <summary>
        /// Get the review type 
        /// </summary>
        /// <param name="reviewTypeId">Review type identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the review type
        /// </returns>
        Task<ReviewType> GetReviewTypeByIdAsync(int reviewTypeId);

        /// <summary>
        /// Insert the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertReviewTypeAsync(ReviewType reviewType);

        /// <summary>
        /// Update the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateReviewTypeAsync(ReviewType reviewType);

        #endregion

        #region TvChannelReviewReviewTypeMapping

        /// <summary>
        /// Get tvchannel review and review type mappings by tvchannel review identifier
        /// </summary>
        /// <param name="tvchannelReviewId">The tvchannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel review and review type mapping collection
        /// </returns>
        Task<IList<TvChannelReviewReviewTypeMapping>> GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(int tvchannelReviewId);

        /// <summary>
        /// Inserts a tvchannel review and review type mapping
        /// </summary>
        /// <param name="tvchannelReviewReviewType">TvChannel review and review type mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelReviewReviewTypeMappingsAsync(TvChannelReviewReviewTypeMapping tvchannelReviewReviewType);

        #endregion
    }
}
