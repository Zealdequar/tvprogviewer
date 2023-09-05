using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Provides an interface for search plugin manager
    /// </summary>
    public interface ISearchPluginManager : IPluginManager<ISearchProvider>
    {
        /// <summary>
        /// Load primary active search provider
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search provider
        /// </returns>
        Task<ISearchProvider> LoadPrimaryPluginAsync(User user = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed search provider is active
        /// </summary>
        /// <param name="searchProvider">Search provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(ISearchProvider searchProvider);

        /// <summary>
        /// Check whether the search provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of search provider to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0);
    }
}