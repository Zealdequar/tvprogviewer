using System.Threading.Tasks;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.WebUI.Models.News;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the news model factory
    /// </summary>
    public partial interface INewsModelFactory
    {
        /// <summary>
        /// Prepare the news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="prepareComments">Whether to prepare news comment models</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the news item model
        /// </returns>
        Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments);

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the home page news items model
        /// </returns>
        Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync();

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the news item list model
        /// </returns>
        Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command);

        /// <summary>
        /// Prepare the news comment model
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the news comment model
        /// </returns>
        Task<NewsCommentModel> PrepareNewsCommentModelAsync(NewsComment newsComment);
    }
}
