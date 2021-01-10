using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement plugin manager implementation
    /// </summary>
    public partial class DiscountPluginManager : PluginManager<IDiscountRequirementRule>, IDiscountPluginManager
    {
        #region Ctor

        public DiscountPluginManager(IPluginService pluginService) : base(pluginService)
        {
        }

        #endregion
    }
}