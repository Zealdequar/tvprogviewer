using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Recently viewed tvchannels service
    /// </summary>
    public partial interface IRecentlyViewedTvChannelsService
    {
        /// <summary>
        /// Gets a "recently viewed tvchannels" list
        /// </summary>
        /// <param name="number">Number of tvchannels to load</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "recently viewed tvchannels" list
        /// </returns>
        Task<IList<TvChannel>> GetRecentlyViewedTvChannelsAsync(int number);

        /// <summary>
        /// Adds a tvchannel to a recently viewed tvchannels list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddTvChannelToRecentlyViewedListAsync(int tvchannelId);
    }
}
