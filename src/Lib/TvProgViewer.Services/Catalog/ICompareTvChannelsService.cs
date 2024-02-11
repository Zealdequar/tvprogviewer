using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Compare tvchannels service interface
    /// </summary>
    public partial interface ICompareTvChannelsService
    {
        /// <summary>
        /// Clears a "compare tvchannels" list
        /// </summary>
        void ClearCompareTvChannels();

        /// <summary>
        /// Gets a "compare tvchannels" list
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "Compare tvchannels" list
        /// </returns>
        Task<IList<TvChannel>> GetComparedTvChannelsAsync();

        /// <summary>
        /// Removes a tvchannel from a "compare tvchannels" list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveTvChannelFromCompareListAsync(int tvchannelId);

        /// <summary>
        /// Adds a tvchannel to a "compare tvchannels" list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddTvChannelToCompareListAsync(int tvchannelId);
    }
}
