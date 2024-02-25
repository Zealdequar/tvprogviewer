using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Home;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory
    /// </summary>
    public partial interface IHomeModelFactory
    {
        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the dashboard model
        /// </returns>
        Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model);

        /// <summary>
        /// Prepare tvProgViewer news model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvProgViewer news model
        /// </returns>
        Task<TvProgViewerNewsModel> PrepareTvProgViewerNewsModelAsync();
    }
}