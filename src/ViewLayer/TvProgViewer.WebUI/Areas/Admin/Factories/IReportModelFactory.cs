using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Reports;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the report model factory
    /// </summary>
    public partial interface IReportModelFactory
    {
        #region Sales summary

        /// <summary>
        /// Prepare sales summary search model
        /// </summary>
        /// <param name="searchModel">Sales summary search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sales summary search model
        /// </returns>
        Task<SalesSummarySearchModel> PrepareSalesSummarySearchModelAsync(SalesSummarySearchModel searchModel);

        /// <summary>
        /// Prepare sales summary list model
        /// </summary>
        /// <param name="searchModel">Sales summary search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sales summary list model
        /// </returns>
        Task<SalesSummaryListModel> PrepareSalesSummaryListModelAsync(SalesSummarySearchModel searchModel);

        #endregion

        #region LowStockTvChannel

        /// <summary>
        /// Prepare low stock tvChannel search model
        /// </summary>
        /// <param name="searchModel">Low stock tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the low stock tvChannel search model
        /// </returns>
        Task<LowStockTvChannelSearchModel> PrepareLowStockTvChannelSearchModelAsync(LowStockTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged low stock tvChannel list model
        /// </summary>
        /// <param name="searchModel">Low stock tvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the low stock tvChannel list model
        /// </returns>
        Task<LowStockTvChannelListModel> PrepareLowStockTvChannelListModelAsync(LowStockTvChannelSearchModel searchModel);

        #endregion

        #region Bestseller

        /// <summary>
        /// Prepare bestseller search model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the bestseller search model
        /// </returns>
        Task<BestsellerSearchModel> PrepareBestsellerSearchModelAsync(BestsellerSearchModel searchModel);

        /// <summary>
        /// Prepare paged bestseller list model
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the bestseller list model
        /// </returns>
        Task<BestsellerListModel> PrepareBestsellerListModelAsync(BestsellerSearchModel searchModel);

        /// <summary>
        /// Get bestsellers total amount
        /// </summary>
        /// <param name="searchModel">Bestseller search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the bestseller total amount
        /// </returns>
        Task<string> GetBestsellerTotalAmountAsync(BestsellerSearchModel searchModel);

        #endregion

        #region NeverSold

        /// <summary>
        /// Prepare never sold report search model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the never sold report search model
        /// </returns>
        Task<NeverSoldReportSearchModel> PrepareNeverSoldSearchModelAsync(NeverSoldReportSearchModel searchModel);

        /// <summary>
        /// Prepare paged never sold report list model
        /// </summary>
        /// <param name="searchModel">Never sold report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the never sold report list model
        /// </returns>
        Task<NeverSoldReportListModel> PrepareNeverSoldListModelAsync(NeverSoldReportSearchModel searchModel);

        #endregion

        #region Country sales

        /// <summary>
        /// Prepare country report search model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the country report search model
        /// </returns>
        Task<CountryReportSearchModel> PrepareCountrySalesSearchModelAsync(CountryReportSearchModel searchModel);

        /// <summary>
        /// Prepare paged country report list model
        /// </summary>
        /// <param name="searchModel">Country report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the country report list model
        /// </returns>
        Task<CountryReportListModel> PrepareCountrySalesListModelAsync(CountryReportSearchModel searchModel);

        #endregion

        #region User reports

        /// <summary>
        /// Prepare user reports search model
        /// </summary>
        /// <param name="searchModel">User reports search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user reports search model
        /// </returns>
        Task<UserReportsSearchModel> PrepareUserReportsSearchModelAsync(UserReportsSearchModel searchModel);

        /// <summary>
        /// Prepare paged best users report list modelSearchModel searchModel
        /// </summary>
        /// <param name="searchModel">Best users report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the best users report list model
        /// </returns>
        Task<BestUsersReportListModel> PrepareBestUsersReportListModelAsync(BestUsersReportSearchModel searchModel);

        /// <summary>
        /// Prepare paged registered users report list model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the registered users report list model
        /// </returns>
        Task<RegisteredUsersReportListModel> PrepareRegisteredUsersReportListModelAsync(RegisteredUsersReportSearchModel searchModel);

        #endregion
    }
}
