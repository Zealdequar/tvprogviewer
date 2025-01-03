﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Category template service interface
    /// </summary>
    public partial interface ICategoryTemplateService
    {
        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCategoryTemplateAsync(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category templates
        /// </returns>
        Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync();

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template
        /// </returns>
        Task<CategoryTemplate> GetCategoryTemplateByIdAsync(int categoryTemplateId);

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate);
    }
}
