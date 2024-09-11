using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Compare tvChannels service interface
    /// </summary>
    public partial interface ICompareTvChannelsService
    {
        /// <summary>
        /// Clears a "compare tvChannels" list
        /// </summary>
        void ClearCompareTvChannels();

        /// <summary>
        /// Gets a "compare tvChannels" list
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the "Compare tvChannels" list
        /// </returns>
        Task<IList<TvChannel>> GetComparedTvChannelsAsync();

        /// <summary>
        /// Removes a tvChannel from a "compare tvChannels" list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task RemoveTvChannelFromCompareListAsync(int tvChannelId);

        /// <summary>
        /// Adds a tvChannel to a "compare tvChannels" list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddTvChannelToCompareListAsync(int tvChannelId);
    }
}
