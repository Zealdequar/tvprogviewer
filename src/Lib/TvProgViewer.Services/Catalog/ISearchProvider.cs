using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Provides an interface for creating search provider
    /// </summary>
    public interface ISearchProvider : IPlugin
    {
        /// <summary>
        /// Get tvchannels identifiers by the specified keywords
        /// </summary>
        /// <param name="keywords">Keywords</param>
        /// <param name="isLocalized">A value indicating whether to search in localized properties</param>
        /// <returns>The task result contains tvchannel identifiers</returns>
        Task<List<int>> SearchTvChannelsAsync(string keywords, bool isLocalized);
    }
}