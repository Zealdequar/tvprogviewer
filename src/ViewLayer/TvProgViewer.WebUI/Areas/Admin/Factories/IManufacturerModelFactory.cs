using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the manufacturer model factory
    /// </summary>
    public partial interface IManufacturerModelFactory
    {
        /// <summary>
        /// Prepare manufacturer search model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer search model
        /// </returns>
        Task<ManufacturerSearchModel> PrepareManufacturerSearchModelAsync(ManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer list model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer list model
        /// </returns>
        Task<ManufacturerListModel> PrepareManufacturerListModelAsync(ManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="model">Manufacturer model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer model
        /// </returns>
        Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model,
            Manufacturer manufacturer, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged manufacturer tvchannel list model
        /// </summary>
        /// <param name="searchModel">Manufacturer tvchannel search model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer tvchannel list model
        /// </returns>
        Task<ManufacturerTvChannelListModel> PrepareManufacturerTvChannelListModelAsync(ManufacturerTvChannelSearchModel searchModel,
            Manufacturer manufacturer);

        /// <summary>
        /// Prepare tvchannel search model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel search model to add to the manufacturer
        /// </returns>
        Task<AddTvChannelToManufacturerSearchModel> PrepareAddTvChannelToManufacturerSearchModelAsync(AddTvChannelToManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel list model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel list model to add to the manufacturer
        /// </returns>
        Task<AddTvChannelToManufacturerListModel> PrepareAddTvChannelToManufacturerListModelAsync(AddTvChannelToManufacturerSearchModel searchModel);
    }
}