using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Tax
{
    /// <summary>
    /// Represents a tax plugin manager
    /// </summary>
    public partial interface ITaxPluginManager : IPluginManager<ITaxProvider>
    {
        /// <summary>
        /// Load primary active tax provider
        /// </summary>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Tax provider</returns>
        ITaxProvider LoadPrimaryPlugin(User User = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed tax provider is active
        /// </summary>
        /// <param name="taxProvider">Tax provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(ITaxProvider taxProvider);

        /// <summary>
        /// Check whether the tax provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of tax provider to check</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(string systemName, User User = null, int storeId = 0);
    }
}