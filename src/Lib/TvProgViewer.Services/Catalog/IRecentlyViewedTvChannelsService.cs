using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Recently viewed tvChannels service
    /// </summary>
    public partial interface IRecentlyViewedTvChannelsService
    {
        /// <summary>
        /// Gets a "recently viewed tvChannels" list
        /// </summary>
        /// <param name="number">Number of tvChannels to load</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the "recently viewed tvChannels" list
        /// </returns>
        Task<IList<TvChannel>> GetRecentlyViewedTvChannelsAsync(int number);

        /// <summary>
        /// Adds a tvChannel to a recently viewed tvChannels list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddTvChannelToRecentlyViewedListAsync(int tvChannelId);
    }
}
