﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;

namespace TvProgViewer.Services.Directory
{
    /// <summary>
    /// Measure dimension service interface
    /// </summary>
    public partial interface IMeasureService
    {
        /// <summary>
        /// Deletes measure dimension
        /// </summary>
        /// <param name="measureDimension">Measure dimension</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteMeasureDimensionAsync(MeasureDimension measureDimension);

        /// <summary>
        /// Gets a measure dimension by identifier
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure dimension
        /// </returns>
        Task<MeasureDimension> GetMeasureDimensionByIdAsync(int measureDimensionId);

        /// <summary>
        /// Gets a measure dimension by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure dimension
        /// </returns>
        Task<MeasureDimension> GetMeasureDimensionBySystemKeywordAsync(string systemKeyword);

        /// <summary>
        /// Gets all measure dimensions
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure dimensions
        /// </returns>
        Task<IList<MeasureDimension>> GetAllMeasureDimensionsAsync();

        /// <summary>
        /// Inserts a measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertMeasureDimensionAsync(MeasureDimension measure);

        /// <summary>
        /// Updates the measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateMeasureDimensionAsync(MeasureDimension measure);

        /// <summary>
        /// Converts dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertDimensionAsync(decimal value,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension, bool round = true);

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertFromPrimaryMeasureDimensionAsync(decimal value,
            MeasureDimension targetMeasureDimension);

        /// <summary>
        /// Deletes measure weight
        /// </summary>
        /// <param name="measureWeight">Measure weight</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteMeasureWeightAsync(MeasureWeight measureWeight);

        /// <summary>
        /// Gets a measure weight by identifier
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure weight
        /// </returns>
        Task<MeasureWeight> GetMeasureWeightByIdAsync(int measureWeightId);

        /// <summary>
        /// Gets a measure weight by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure weight
        /// </returns>
        Task<MeasureWeight> GetMeasureWeightBySystemKeywordAsync(string systemKeyword);

        /// <summary>
        /// Gets all measure weights
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the measure weights
        /// </returns>
        Task<IList<MeasureWeight>> GetAllMeasureWeightsAsync();

        /// <summary>
        /// Inserts a measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertMeasureWeightAsync(MeasureWeight measure);

        /// <summary>
        /// Updates the measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateMeasureWeightAsync(MeasureWeight measure);

        /// <summary>
        /// Converts weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertWeightAsync(decimal value,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight, bool round = true);

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertFromPrimaryMeasureWeightAsync(decimal value,
            MeasureWeight targetMeasureWeight);

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertToPrimaryMeasureDimensionAsync(decimal value,
            MeasureDimension sourceMeasureDimension);

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the converted value
        /// </returns>
        Task<decimal> ConvertToPrimaryMeasureWeightAsync(decimal value, MeasureWeight sourceMeasureWeight);
    }
}