using TVProgViewer.Services.Plugins;
using TVProgViewer.Services.Users;

namespace TVProgViewer.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement plugin manager implementation
    /// </summary>
    public partial class DiscountPluginManager : PluginManager<IDiscountRequirementRule>, IDiscountPluginManager
    {
        #region Ctor

        public DiscountPluginManager(IUserService userService, IPluginService pluginService) : base(userService, pluginService)
        {
        }

        #endregion
    }
}