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
        /// Delete a tvChannel tag
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelTagAsync(TvChannelTag tvChannelTag);

        /// <summary>
        /// Delete tvChannel tags
        /// </summary>
        /// <param name="tvChannelTags">TvChannel tags</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelTagsAsync(IList<TvChannelTag> tvChannelTags);

        /// <summary>
        /// Gets tvChannel tags
        /// </summary>
        /// <param name="tvChannelTagIds">TvChannel tags identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetTvChannelTagsByIdsAsync(int[] tvChannelTagIds);

        /// <summary>
        /// Gets all tvChannel tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetAllTvChannelTagsAsync(string tagName = null);

        /// <summary>
        /// Gets all tvChannel tags by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags
        /// </returns>
        Task<IList<TvChannelTag>> GetAllTvChannelTagsByTvChannelIdAsync(int tvChannelId);

        /// <summary>
        /// Gets tvChannel tag
        /// </summary>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag
        /// </returns>
        Task<TvChannelTag> GetTvChannelTagByIdAsync(int tvChannelTagId);

        /// <summary>
        /// Inserts a tvchannel-tvchannel tag mapping
        /// </summary>
        /// <param name="tagMapping">TvChannel-tvchannel tag mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelTvChannelTagMappingAsync(TvChannelTvChannelTagMapping tagMapping);
        
        /// <summary>
        /// Updates the tvChannel tag
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelTagAsync(TvChannelTag tvChannelTag);

        /// <summary>
        /// Get number of tvChannels
        /// </summary>
        /// <param name="tvChannelTagId">TvChannel tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        Task<int> GetTvChannelCountByTvChannelTagIdAsync(int tvChannelTagId, int storeId, bool showHidden = false);

        /// <summary>
        /// Get tvChannel count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the dictionary of "tvChannel tag ID : tvChannel count"
        /// </returns>
        Task<Dictionary<int, int>> GetTvChannelCountAsync(int storeId, bool showHidden = false);
        
        /// <summary>
        /// Update tvChannel tags
        /// </summary>
        /// <param name="tvChannel">TvChannel for update</param>
        /// <param name="tvChannelTags">TvChannel tags</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelTagsAsync(TvChannel tvChannel, string[] tvChannelTags);
    }
}
