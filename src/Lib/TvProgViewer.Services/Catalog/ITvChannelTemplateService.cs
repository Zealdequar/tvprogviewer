using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel template interface
    /// </summary>
    public partial interface ITvChannelTemplateService
    {
        /// <summary>
        /// Delete tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate);

        /// <summary>
        /// Gets all tvChannel templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel templates
        /// </returns>
        Task<IList<TvChannelTemplate>> GetAllTvChannelTemplatesAsync();

        /// <summary>
        /// Gets a tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplateId">TvChannel template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel template
        /// </returns>
        Task<TvChannelTemplate> GetTvChannelTemplateByIdAsync(int tvChannelTemplateId);

        /// <summary>
        /// Inserts tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate);

        /// <summary>
        /// Updates the tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate);
    }
}
