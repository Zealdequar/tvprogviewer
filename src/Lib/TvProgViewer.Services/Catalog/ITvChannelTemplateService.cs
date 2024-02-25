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
        /// Delete tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate);

        /// <summary>
        /// Gets all tvchannel templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel templates
        /// </returns>
        Task<IList<TvChannelTemplate>> GetAllTvChannelTemplatesAsync();

        /// <summary>
        /// Gets a tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplateId">TvChannel template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel template
        /// </returns>
        Task<TvChannelTemplate> GetTvChannelTemplateByIdAsync(int tvchannelTemplateId);

        /// <summary>
        /// Inserts tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate);

        /// <summary>
        /// Updates the tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate);
    }
}
