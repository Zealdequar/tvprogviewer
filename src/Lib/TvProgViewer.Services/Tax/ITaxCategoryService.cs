﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Tax;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Tax category service interface
    /// </summary>
    public partial interface ITaxCategoryService
    {
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax categories
        /// </returns>
        Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax category
        /// </returns>
        Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
    }
}
