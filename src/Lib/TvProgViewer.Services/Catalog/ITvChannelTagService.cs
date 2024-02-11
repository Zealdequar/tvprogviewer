using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel tag service interface
    /// </summary>
    public partial interface ITvChannelTagService
    {
        /// <summary>
        /// Delete a tvchannel tag
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelTagAsync(TvChannelTag tvchannelTag);

        /// <summary>
        /// Delete tvchannel tags
        /// </summary>
        /// <param name="tvchannelTags">TvChannel tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelTagsAsync(IList<TvChannelTag> tvchannelTags);

        /// <summary>
        /// Gets tvchannel tags
        /// </summary>
        /// <param name="tvchannelTagIds">TvChannel tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetTvChannelTagsByIdsAsync(int[] tvchannelTagIds);

        /// <summary>
        /// Gets all tvchannel tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetAllTvChannelTagsAsync(string tagName = null);

        /// <summary>
        /// Gets all tvchannel tags by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetAllTvChannelTagsByTvChannelIdAsync(int tvchannelId);

        /// <summary>
        /// Gets tvchannel tag
        /// </summary>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag
        /// </returns>
        Task<TvChannelTag> GetTvChannelTagByIdAsync(int tvchannelTagId);

        /// <summary>
        /// Inserts a tvchannel-tvchannel tag mapping
        /// </summary>
        /// <param name="tagMapping">TvChannel-tvchannel tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelTvChannelTagMappingAsync(TvChannelTvChannelTagMapping tagMapping);
        
        /// <summary>
        /// Updates the tvchannel tag
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelTagAsync(TvChannelTag tvchannelTag);

        /// <summary>
        /// Get number of tvchannels
        /// </summary>
        /// <param name="tvchannelTagId">TvChannel tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of tvchannels
        /// </returns>
        Task<int> GetTvChannelCountByTvChannelTagIdAsync(int tvchannelTagId, int storeId, bool showHidden = false);

        /// <summary>
        /// Get tvchannel count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary of "tvchannel tag ID : tvchannel count"
        /// </returns>
        Task<Dictionary<int, int>> GetTvChannelCountAsync(int storeId, bool showHidden = false);
        
        /// <summary>
        /// Update tvchannel tags
        /// </summary>
        /// <param name="tvchannel">TvChannel for update</param>
        /// <param name="tvchannelTags">TvChannel tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelTagsAsync(TvChannel tvchannel, string[] tvchannelTags);
    }
}
