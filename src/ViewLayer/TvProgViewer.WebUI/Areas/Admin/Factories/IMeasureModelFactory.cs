﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Directory;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the measures model factory
    /// </summary>
    public partial interface IMeasureModelFactory
    {
        /// <summary>
        /// Prepare measure search model
        /// </summary>
        /// <param name="searchModel">Measure search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure search model
        /// </returns>
        Task<MeasureSearchModel> PrepareMeasureSearchModelAsync(MeasureSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure dimension list model
        /// </summary>
        /// <param name="searchModel">Measure dimension search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure dimension list model
        /// </returns>
        Task<MeasureDimensionListModel> PrepareMeasureDimensionListModelAsync(MeasureDimensionSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure weight list model
        /// </summary>
        /// <param name="searchModel">Measure weight search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure weight list model
        /// </returns>
        Task<MeasureWeightListModel> PrepareMeasureWeightListModelAsync(MeasureWeightSearchModel searchModel);
    }
}