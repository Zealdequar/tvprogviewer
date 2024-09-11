using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel review model factory
    /// </summary>
    public partial interface ITvChannelReviewModelFactory
    {
        #region ProducrReview

        /// <summary>
        /// Prepare tvChannel review search model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review search model
        /// </returns>
        Task<TvChannelReviewSearchModel> PrepareTvChannelReviewSearchModelAsync(TvChannelReviewSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel review list model
        /// </summary>
        /// <param name="searchModel">TvChannel review search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review list model
        /// </returns>
        Task<TvChannelReviewListModel> PrepareTvChannelReviewListModelAsync(TvChannelReviewSearchModel searchModel);

        /// <summary>
        /// Prepare tvChannel review model
        /// </summary>
        /// <param name="model">TvChannel review model</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review model
        /// </returns>
        Task<TvChannelReviewModel> PrepareTvChannelReviewModelAsync(TvChannelReviewModel model,
            TvChannelReview tvChannelReview, bool excludeProperties = false);

        #endregion

        #region TvChannelReviewReveiwTypeMapping

        /// <summary>
        /// Prepare paged tvChannel reviews mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel review and review type mapping search model</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review and review type mapping list model
        /// </returns>
        Task<TvChannelReviewReviewTypeMappingListModel> PrepareTvChannelReviewReviewTypeMappingListModelAsync(TvChannelReviewReviewTypeMappingSearchModel searchModel,
            TvChannelReview tvChannelReview);

        #endregion
    }
}