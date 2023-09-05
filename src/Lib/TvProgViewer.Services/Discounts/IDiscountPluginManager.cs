using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement plugin manager
    /// </summary>
    public partial interface IDiscountPluginManager : IPluginManager<IDiscountRequirementRule>
    {
    }
}