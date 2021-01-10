using System.Collections.Generic;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Authentication.External
{
    /// <summary>
    /// Represents an authentication plugin manager implementation
    /// </summary>
    public partial class AuthenticationPluginManager : PluginManager<IExternalAuthenticationMethod>, IAuthenticationPluginManager
    {
        #region Fields

        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

        #endregion

        #region Ctor

        public AuthenticationPluginManager(ExternalAuthenticationSettings externalAuthenticationSettings,
            IPluginService pluginService) : base(pluginService)
        {
            _externalAuthenticationSettings = externalAuthenticationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active authentication methods
        /// </summary>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active authentication methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadActivePlugins(User User = null, int storeId = 0)
        {
            return LoadActivePlugins(_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames, User, storeId);
        }

        /// <summary>
        /// Check whether the passed authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Authentication method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IExternalAuthenticationMethod authenticationMethod)
        {
            return IsPluginActive(authenticationMethod, _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of authentication method to check</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, User User = null, int storeId = 0)
        {
            var authenticationMethod = LoadPluginBySystemName(systemName, User, storeId);
            return IsPluginActive(authenticationMethod);
        }

        #endregion
    }
}