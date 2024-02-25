using System.Threading.Tasks;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Web.Framework.Menu
{
    /// <summary>
    /// Interface for plugins which have some items in the admin area menu
    /// </summary>
    public partial interface IAdminMenuPlugin : IPlugin
    {
        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ManageSiteMapAsync(SiteMapNode rootNode);
    }
}