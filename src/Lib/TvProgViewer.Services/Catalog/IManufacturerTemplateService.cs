using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Manufacturer template service interface
    /// </summary>
    public partial interface IManufacturerTemplateService
    {
        /// <summary>
        /// Delete manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer templates
        /// </returns>
        Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync();

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template
        /// </returns>
        Task<ManufacturerTemplate> GetManufacturerTemplateByIdAsync(int manufacturerTemplateId);

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);
    }
}
