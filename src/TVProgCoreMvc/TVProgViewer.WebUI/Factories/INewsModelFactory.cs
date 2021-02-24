using System.Threading.Tasks;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.WebUI.Models.News;

namespace TVProgViewer.WebUI.Factories
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
        /// <returns>News item model</returns>
        Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments);

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>Home page news items model</returns>
        Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync();

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>News item list model</returns>
        Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command);
    }
}
