using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Represents a search plugin manager implementation
    /// </summary>
    public class SearchPluginManager : PluginManager<ISearchProvider>, ISearchPluginManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public SearchPluginManager(CatalogSettings catalogSettings, IUserService userService, IPluginService pluginService)
                : base(userService, pluginService)
        {
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load primary active search provider
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search provider
        /// </returns>
        public virtual async Task<ISearchProvider> LoadPrimaryPluginAsync(User user = null, int storeId = 0)
        {
            if (string.IsNullOrEmpty(_catalogSettings.ActiveSearchProviderSystemName))
                return null;

            return await LoadPrimaryPluginAsync(_catalogSettings.ActiveSearchProviderSystemName, user, storeId);
        }

        /// <summary>
        /// Check whether the passed search provider is active
        /// </summary>
        /// <param name="searchProvider">Search provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(ISearchProvider searchProvider)
        {
            return IsPluginActive(searchProvider, new List<string> { _catalogSettings.ActiveSearchProviderSystemName });
        }

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
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var searchProvider = await LoadPluginBySystemNameAsync(systemName, user, storeId);
            return IsPluginActive(searchProvider);
        }

        #endregion
    }
}